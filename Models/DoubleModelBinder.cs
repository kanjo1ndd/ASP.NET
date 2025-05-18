using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ASP_SPR311.Models
{
	//public class DoubleModelBinder : IModelBinder
	//{
	//	public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
	//	{
	//		var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
	//		if (result != null && !string.IsNullOrEmpty(result.AttemptedValue))
	//		{
	//			if (bindingContext.ModelType == typeof(double))
	//			{
	//				double temp;
	//				var attempted = result.AttemptedValue.Replace(",", ".");
	//				if (double.TryParse(
	//					attempted,
	//					NumberStyles.Number,
	//					CultureInfo.InvariantCulture,
	//					out temp)
	//				)
	//				{
	//					return temp;
	//				}
	//			}
	//		}
	//		return base.BindModel(controllerContext, bindingContext);
	//	}
	//}
}
