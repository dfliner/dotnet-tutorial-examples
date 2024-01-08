using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using WestPacificUniversity.Utilities;

namespace WestPacificUniversity.EFCore.Repositories;

public static class QueryableExtensions
{
    /// <summary>
    /// Filters query results based on <paramref name="regex"/> match in all string properties
    /// of the query source.
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="query"/></typeparam>
    /// <param name="query">The query source</param>
    /// <param name="regex">The regular expression that defines the text match criteria</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that contains the elements that satisfy the filtering criteria.
    /// </returns>
    public static IQueryable<T> TextFilter<T>(this IQueryable<T> query, string regex)
    {
        return TextFilter(query, regex, (PropertyInfo[]?)null);
    }

    /// <summary>
    /// Filters query results based on <paramref name="regex"/> match in specified properties
    /// of the query source.
    /// </summary>
    /// <typeparam name="T">The type of the elements in <paramref name="query"/></typeparam>
    /// <param name="query">The query source</param>
    /// <param name="regex">The regular expression that defines the text match criteria</param>
    /// <param name="propertyNames">An array of names of specified properties where match is tested.</param>
    /// <returns>
    /// An <see cref="IQueryable{T}"/> that contains the elements that satisfy the filtering criteria.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// If the value of any of the properties is not of string type.
    /// </exception>
    public static IQueryable<T> TextFilter<T>(this IQueryable<T> query, string regex, params string[]? propertyNames)
    {
        if (propertyNames == null || propertyNames.Length == 0)
        {
            // No property
            return query;
        }

        PropertyInfo[] properties = propertyNames
            .Select(prop =>
            {
                return
                    typeof(T)
                    .GetProperty(prop, typeof(string))
                    ?? throw new InvalidOperationException($"{prop} is not a string property");
            })
            .ToArray();

        return TextFilter(query, regex, properties);
    }

    /// <summary>
    /// Sorts query results on the properties in specified order respectively.
    /// </summary>
    /// <typeparam name="T">The type of elements in <paramref name="query"/></typeparam>
    /// <param name="query">The query source</param>
    /// <param name="propertyAndSortOrders">An array of property and its sorting order.</param>
    /// <returns>An <see cref="IOrderedQueryable{T}"/> whose results are sorted.</returns>
    /// <remarks>Property paths for navigation properties are dot ('.') separated.</remarks>
    /// <exception cref="InvalidOperationException">
    /// If any of the sort criteria (property path or sorting order) is invalid.
    /// </exception>
    public static IOrderedQueryable<T> OrderBy<T>(
        this IQueryable<T> query,
        params (string propertyPath, string sortOrder)[] propertyAndSortOrders)
    {
        CheckArgument.ThrowIfNull(query, nameof(query));

        if (propertyAndSortOrders == null || propertyAndSortOrders.Length == 0)
        {
            throw new InvalidOperationException($"Must specify at least one sort criteria");
        }

        bool first = true; // The first order-by statement
        foreach (var (propertyPath, sortOrder) in propertyAndSortOrders)
        {
            CheckArgument.ThrowIfNullOrWhiteSpace(propertyPath, nameof(propertyPath));
            CheckArgument.ThrowIfNullOrWhiteSpace(sortOrder, nameof(sortOrder));

            MethodInfo sortMethodInfo = null;
            switch (sortOrder.Trim().ToLowerInvariant())
            {
                case "asc":
                    sortMethodInfo = first ? OrderByMethodInfo : ThenByMethodInfo;
                    break;
                case "desc":
                    sortMethodInfo = first ? OrderByDescendingMethodInfo : ThenByDescendingMethodInfo;
                    break;
                default:
                    throw new InvalidOperationException($"Unknown sort order: {sortOrder}");
            }

            query = ApplySortOrder(query, propertyPath, sortMethodInfo);
            first = false;
        }

        return (query as IOrderedQueryable<T>)!;
    }

    private static IOrderedQueryable<T> ApplySortOrder<T>(
        IQueryable<T> query,
        string propertyPath,
        MethodInfo sortMethodInfo)
    {
        Type type = typeof(T);

        // The 'x' in 'x => x.NavigationProperty.PropertyName'
        ParameterExpression arg = Expression.Parameter(type, "x");

        // Constructs the expression for property accessor (x.NavigationProperty.PropertyName)
        Expression propertyAccessor = arg;
        string[] props = propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var prop in props)
        {
            PropertyInfo? propertyInfo = type.GetProperty(prop);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException($"{type.Name} does not have a property named {prop}");
            }

            propertyAccessor = Expression.Property(propertyAccessor, propertyInfo);
            type = propertyInfo.PropertyType;
        }

        // keySelector lambda in the sort method: x => x.NavigationProperty.PropertyName
        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
        LambdaExpression keySelector = Expression.Lambda(delegateType, propertyAccessor, arg);

        // Invoke the sort method
        var methodInfo = sortMethodInfo.MakeGenericMethod(typeof(T), type);
        return (IOrderedQueryable<T>)methodInfo.Invoke(null, new object[] { query, keySelector })!;
    }

    private static IQueryable<T> TextFilter<T>(IQueryable<T> query, string regex, params PropertyInfo[]? properties)
    {
        CheckArgument.ThrowIfNull(query, nameof(query));

        if (string.IsNullOrWhiteSpace(regex))
        {
            return query;
        }

        if (properties == null)
        {
            // All public string properties on the type of the query source
            properties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(pi => pi.PropertyType == typeof(string))
                .ToArray();
        }

        ParameterExpression xArg = Expression.Parameter(typeof(T), "x");

        // Constructs an expression for each property:
        // Regex.IsMatch(x.PropertyName, regex, options)
        IEnumerable<Expression> expressions = properties
            .Select(prop =>
            {
                if (prop.PropertyType != typeof(string))
                {
                    throw new InvalidOperationException($"{prop.Name} is not a string property");
                }

                return Expression.Call(
                    RegexIsMatchMethodInfo,
                    Expression.Property(xArg, prop),
                    Expression.Constant(regex),
                    Expression.Constant(RegexOptions.IgnoreCase));
            });

        // Combines all resultant expressions using "||"
        Expression body = expressions.Aggregate((prev, current) => Expression.OrElse(prev, current));

        // x => Regex.IsMatch(x.PropertyName1, regex, options) ||
        //      Regex.IsMatch(x.PropertyName2, regex, options) || ...
        Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(body, xArg);

        return query.Where(predicate);
    }

    // The difference between overloads are the count of parameters
    private static readonly MethodInfo OrderByMethodInfo =
        typeof(Queryable)
        .GetTypeInfo()
        .GetDeclaredMethods("OrderBy")
        .Single(mi => mi.GetParameters().Length == 2);

    private static readonly MethodInfo OrderByDescendingMethodInfo =
        typeof(Queryable)
        .GetTypeInfo()
        .GetDeclaredMethods("OrderByDescending")
        .Single(mi => mi.GetParameters().Length == 2);

    private static readonly MethodInfo ThenByMethodInfo =
        typeof(Queryable)
        .GetTypeInfo()
        .GetDeclaredMethods("ThenBy")
        .Single(mi => mi.GetParameters().Length == 2);

    private static readonly MethodInfo ThenByDescendingMethodInfo =
        typeof(Queryable)
        .GetTypeInfo()
        .GetDeclaredMethods("ThenByDescending")
        .Single(mi => mi.GetParameters().Length == 2);

    private static readonly MethodInfo RegexIsMatchMethodInfo =
        typeof(Regex)
        .GetMethod("IsMatch",
            BindingFlags.Static | BindingFlags.Public,
            new[] { typeof(string), typeof(string), typeof(RegexOptions) }
        )!;
}
