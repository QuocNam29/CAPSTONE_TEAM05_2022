using CAP_TEAM05_2022.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;



namespace CAP_TEAM05_2022.Controllers.Tests
{
    [TestClass()]
    public class CategoryController
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
        public void Check_False_If_Name_Null_Test()
        {
            // Arrange
            var controller = new categoriesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new category()
            {
                name = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Vui lòng nhập tên danh mục !")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_Name_MoreThan_255_Characters_Test()
        {
            // Arrange
            var controller = new categoriesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new category()
            {
                name = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Name length must be between 1 and 255.")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_code_Null_Test()
        {
            // Arrange
            var controller = new categoriesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new category()
            {
                code = null
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Category code cannot be empty !")).Count() > 0);
        }
        [TestMethod()]
        public void Check_False_If_code_MoreThan_50_Characters_Test()
        {
            // Arrange
            var controller = new categoriesController();
            var db = new CP25Team05Entities();

            // Act          
            var model = new category()
            {
                code = "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111"
            };

            // Assert
            Assert.IsTrue(ValidateModel(model).Where(x => x.ErrorMessage.Equals("Code length must be between 1 and 50.")).Count() > 0);
        }




    }
}