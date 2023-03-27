using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfAppDI.Model;

public class User : INotifyPropertyChanged
{
    private int userId;
    private string firstName = default!;
    private string lastName = default!;
    private string email = null!; // also works
    private string city = default!;
    private string state = default!;
    private string country = default!;

    public int UserId
    {
        get { return userId; }
        set
        {
            userId = value;
            OnPropertyChanged(nameof(UserId));
        }
    }

    public string FirstName
    { 
        get { return firstName; } 
        set { 
            firstName = value; 
            OnPropertyChanged(nameof(FirstName));
        }
    }

    public string LastName
    {
        get { return lastName; }
        set
        {
            lastName = value;
            OnPropertyChanged(nameof(LastName));
        }
    }

    public string Email
    {
        get { return email; }
        set
        {
            email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    public string City
    {
        get { return city; }
        set
        {
            city = value;
            OnPropertyChanged(nameof(City));
        }
    }

    public string State
    {
        get { return state; }
        set
        {
            state = value;
            OnPropertyChanged(nameof(State));
        }
    }

    public string Country
    {
        get { return country; }
        set
        {
            country = value;
            // nameof(Country) is automatically used to invoke the call.
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Use the caller method/property name if <paramref name="propertyName"/> is not specified.
    /// </summary>
    /// <param name="propertyName">The name of the property that triggers this event.</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
