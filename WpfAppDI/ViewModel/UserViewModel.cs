using Castle.Windsor;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using WpfAppDI.Model;
using WpfAppDI.StartupHelpers;

namespace WpfAppDI.ViewModel;

public class UserViewModel
{
    private readonly IList<User> _users;
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IWindsorContainer _container;

    public UserViewModel(IDialogCoordinator dialogCoordinator, IWindsorContainer container)
    {
        Debug.Assert(container == WindsorBootstrapper.Container);

        _dialogCoordinator = dialogCoordinator;
        _container = container;

        _users = new List<User>()
        {
            // Example comes from https://www.c-sharpcorner.com/UploadFile/raj1979/simple-mvvm-pattern-in-wpf/
            new User{UserId=1,FirstName="Raj",LastName="Beniwal",City="Delhi",State="DEL",Country="INDIA"},
            new User{UserId=2,FirstName="Mark",LastName="henry",City="New York", State="NY", Country="USA"},
            new User{UserId=3,FirstName="Mahesh",LastName="Chand",City="Philadelphia", State="PHL", Country="USA"},
            new User{UserId=4,FirstName="Vikash",LastName="Nanda",City="Noida", State="UP", Country="INDIA"},
            new User{UserId=5,FirstName="Harsh",LastName="Kumar",City="Ghaziabad", State="UP", Country="INDIA"},
            new User{UserId=6,FirstName="Reetesh",LastName="Tomar",City="Mumbai", State="MP", Country="INDIA"},
            new User{UserId=7,FirstName="Deven",LastName="Verma",City="Palwal", State="HP", Country="INDIA"},
            new User{UserId=8,FirstName="Ravi",LastName="Taneja",City="Delhi", State="DEL", Country="INDIA"}
        };
    }

    public IList<User> Users
    {
        get { return _users; }
        //set { _users = value; }
    }

    private ICommand? mUpdater;
    public ICommand UpdateCommand => mUpdater ?? (mUpdater = new UpdateCommand());
}

class UpdateCommand : CommandBase
{
    public override bool CanExecute(object? parameter)
    {
        return true;
    }

    public override void Execute(object? parameter)
    {
    }
}