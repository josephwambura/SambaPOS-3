﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using Samba.Domain.Models.Tickets;
using Samba.Domain.Models.Users;
using Samba.Infrastructure.Data;
using Samba.Infrastructure.Data.Validation;
using Samba.Localization.Properties;
using Samba.Persistance.Data;

namespace Samba.Persistance.Implementations
{
    [Export(typeof(IUserDao))]
    class UserDao : IUserDao
    {
        [ImportingConstructor]
        public UserDao()
        {
            ValidatorRegistry.RegisterDeleteValidator(new UserDeleteValidator());
            ValidatorRegistry.RegisterDeleteValidator(new UserRoleDeleteValidator());
            ValidatorRegistry.RegisterSaveValidator(new UserSaveValidator());
        }

        public bool GetIsUserExists(string pinCode)
        {
            return Dao.Exists<User>(x => x.PinCode == pinCode);
        }

        public User GetUserByPinCode(string pinCode)
        {
            return Dao.Single<User>(x => x.PinCode == pinCode, x => x.UserRole.Permissions);
        }

        public IEnumerable<UserRole> GetUserRoles()
        {
            return Dao.Query<UserRole>();
        }
    }

    public class UserSaveValidator : SpecificationValidator<User>
    {
        public override string GetErrorMessage(User model)
        {
            return Dao.Exists<User>(x => x.PinCode == model.PinCode && x.Id != model.Id) ? Resources.SaveErrorThisPinCodeInUse : "";
        }
    }

    public class UserDeleteValidator : SpecificationValidator<User>
    {
        public override string GetErrorMessage(User model)
        {
            return model.UserRole.IsAdmin
                ? Resources.DeleteErrorAdminUser
                : Dao.Count<User>() == 1
                ? Resources.DeleteErrorLastUser
                : Dao.Exists<Order>(x => x.CreatingUserName == model.Name)
                ? string.Format(Resources.DeleteErrorUsedBy_f, Resources.User, Resources.Order)
                : "";
        }
    }

    public class UserRoleDeleteValidator : SpecificationValidator<UserRole>
    {
        public override string GetErrorMessage(UserRole model)
        {
            return model.Id == 1 || model.IsAdmin
                ? string.Format(Resources.CantDelete_f, Resources.AdminRole)
                : Dao.Exists<User>(y => y.UserRole.Id == model.Id)
                ? string.Format(Resources.DeleteErrorUsedBy_f, Resources.UserRole, Resources.User)
                : "";
        }
    }
}
