using CAP_TEAM05_2022.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CAP_TEAM05_2022.Controllers.Tests
{
    [TestClass]
    public class usersController
    {
        public class MockHttpSession : HttpSessionStateBase
        {
            public Hashtable buffer = new Hashtable();
            public override object this[string key]
            {
                get
                {
                    return buffer[key];
                }
                set
                {
                    buffer[key] = value;
                }
            }
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [TestMethod()]
        public void Check_False_If_Name_User_Null_Test()
        {
            // Arrange
            var controller = new usersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new user()
            {
                name = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập họ và tên !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Name_User_MoreThan_255_Characters_Test()
        {
            // Arrange
            var controller = new usersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new user()
            {
                name = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Nhập họ và tên phải dưới 255 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Email_MoreThan_255_Characters_Test()
        {
            // Arrange
            var controller = new usersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new user()
            {
                email = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Nhập Email phải dưới 255 ký tự")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Phone_User_Null_Test()
        {
            // Arrange
            var controller = new usersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new user()
            {
                phone = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập số điện thoại !")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_Phone_More_Than_11_Characters_Test()
        {
            // Arrange
            var controller = new usersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new user()
            {
                phone = "012345678910"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Sai định dạng số điện thoại")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Address_Customer_Null_Test()
        {
            // Arrange
            var controller = new usersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new user()
            {
                address = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập địa chỉ !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Address_Customer__More_Than_255_Characters_Test()
        {
            // Arrange
            var controller = new usersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new user()
            {
                address = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Nhập địa chỉ phải dưới 255 ký tự.")).Count() > 0);
        }
    }
}
