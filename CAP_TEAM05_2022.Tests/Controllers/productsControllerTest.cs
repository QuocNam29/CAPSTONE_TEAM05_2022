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
    public class productsControllerTest
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
        public void Check_False_If_Name_Product_Null_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                name = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập tên sản phẩm !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Name_Product_MoreThan_255_Characters_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                name = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Name length must be between 1 and 255.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Code_Null_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                code = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Product code cannot be empty !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Code_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                code = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Code length must be between 1 and 100.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Unit_Null_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                unit = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập tên đơn vị tính !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Unit_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                unit = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Unit length must be between 1 and 100.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_purchase_price_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                purchase_price = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Please enter a value bigger than 0")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_sell_price_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                sell_price = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Please enter a value bigger than 0")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_quantity_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new productsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new product()
            {
                quantity = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Please enter a value bigger than 0")).Count() > 0);
        }
    }
}
