﻿using CAP_TEAM05_2022.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CAP_TEAM05_2022.Controllers.Tests
{
    [TestClass]
    public class customersControllerTest
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
        public void Check_False_If_Name_Customer_Null_Test()
        {
            // Arrange
            var controller = new customersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new customer()
            {
                name = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập họ và tên !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Name_Customer_MoreThan_255_Characters_Test()
        {
            // Arrange
            var controller = new customersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new customer()
            {
                name = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Họ và tên tối đa 255 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Email_MoreThan_255_Characters_Test()
        {
            // Arrange
            var controller = new customersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new customer()
            {
                email = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Email tối đa 255 ký tự")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Phone_Customer_Null_Test()
        {
            // Arrange
            var controller = new customersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new customer()
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
            var controller = new customersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new customer()
            {
                phone = "012345678910"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Address_Customer_Null_Test()
        {
            // Arrange
            var controller = new customersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new customer()
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
            var controller = new customersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new customer()
            {
                address = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Địa chỉ không quá 255 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Type_Customer_Null_Test()
        {
            // Arrange
            var controller = new customersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new customer()
            {
                type = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Count() > 0).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_AccountNumber_Customer__More_Than_20_Characters_Test()
        {
            // Arrange
            var controller = new customersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new customer()
            {
                account_number = "012345678910111121314151617181920"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Số tài khoản không quá 20 kí tự")).Count() > 0);
        }
        [TestMethod()]

        public void Check_False_If_Bank_Customer__More_Than_255_Characters_Test()
        {
            // Arrange
            var controller = new customersController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new customer()
            {
                bank = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Tên ngân hàng phải dưới 255 ký tự.")).Count() > 0);
        }
    }
}
