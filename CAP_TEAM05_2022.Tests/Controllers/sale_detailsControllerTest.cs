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
    public class sale_detailsControllerTest
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
        public void Check_False_If_Unit_swap_Null_Test()
        {
            // Arrange
            var controller = new sale_detailsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale_details()
            {
                unit = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập đơn vị tính !")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Unit_MoreThan_100_Characters_Test()
        {
            // Arrange
            var controller = new sale_detailsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale_details()
            {
                unit = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Đơn vị phải dưới 100 ký tự.")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Price_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new sale_detailsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale_details()
            {
                price = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập đơn giá phải lớn hơn 0")).Count() > 0);
        }

        [TestMethod()]
        public void Check_False_If_Sold_Value_bigger_than_0_Test()
        {
            // Arrange
            var controller = new sale_detailsController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new sale_details()
            {
                sold = -2
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập giá bán phải lớn hơn 0")).Count() > 0);
        }
    }
}
