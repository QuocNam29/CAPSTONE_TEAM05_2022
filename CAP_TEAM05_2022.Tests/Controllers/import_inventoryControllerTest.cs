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
    public class import_inventoryControllerTest
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
        public void Check_False_If_quantity_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new import_inventoryController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new import_inventory()
            {
                quantity = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập số lượng lớn hơn 0")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_sold_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new import_inventoryController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new import_inventory()
            {
                sold = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập đơn giá bán phải lớn hơn 0")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Price_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new import_inventoryController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new import_inventory()
            {
                price_import = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập đơn giá nhập phải lớn hơn 0")).Count() > 0);
        }
    }
}
