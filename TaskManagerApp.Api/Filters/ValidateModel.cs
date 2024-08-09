using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace TaskManagerApp.Api.Filters
{
    public class ValidateModel : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }

            var model = context.ActionArguments.Values.FirstOrDefault();
            if (model == null)
            {
                context.Result = new BadRequestObjectResult("model can not be null");
                return;
            }

            var validationResults = ValidateModelProperties(model);
            if (validationResults.Any())
            {
                context.Result = new BadRequestObjectResult(validationResults);
            }
        }

        private static List<ValidationResult> ValidateModelProperties(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, serviceProvider: null, items: null);

            foreach (var property in model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(model);
                var validationAttributes = property.GetCustomAttributes<ValidationAttribute>();

                foreach (var attribute in validationAttributes)
                {
                    if (value != null && !attribute.IsValid(value))
                    {
                        validationResults.Add(new ValidationResult(attribute.ErrorMessage, new[] { property.Name }));
                    }
                }
            }

            return validationResults;
        }
    }
}