using CAP_TEAM05_2022.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using CAP_TEAM05_2022.Models;
using Moq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

namespace CAP_TEAM05_2022.Controllers.Tests    
{
    [TestClass]
    public class salesControllerTest
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
        public void Check_False_If_Code_Null_Test()
        {
            // Arrange
            var controller = new salesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale()
            {
                code = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Sale code cannot be empty !")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_Code_MoreThan_20_Characters_Test()
        {
            // Arrange
            var controller = new salesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale()
            {
                code = "123456789101112131415161718"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Code sale length must be between 1 and 20.")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_total_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new salesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale()
            {
                total = -1
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Please enter a value bigger than -1")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_Discount_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new salesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale()
            {
                discount = -1
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Please enter a value bigger than -1")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_VAT_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new salesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale()
            {
                vat = -1
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Please enter a value bigger than -1")).Count() > 0);
        }
    }
}
