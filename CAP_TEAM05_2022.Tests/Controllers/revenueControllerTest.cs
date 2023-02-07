﻿using CAP_TEAM05_2022.Controllers;
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
    public class revenueControllerTest
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
        public void Check_False_If_Price_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new revenue();
            var db = new CP25Team05Entities();

            // Act          
            var model = new revenue()
            {
                Price = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Please enter a value bigger than 0")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_quantity_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new revenue();
            var db = new CP25Team05Entities();

            // Act          
            var model = new revenue()
            {
                quantity = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Please enter a value bigger than 0")).Count() > 0);
        }
    }
}