using KnockMvc.DTO;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KnockMvc.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MvcHelperGrid()
        {
            var result = new List<MyRow>
            {
                new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 0.234522, EnumValue = MyEnum.Val1, StringValue = "test", NullableDecimalValue = 10 },
                new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf", NullableDecimalValue = null },
                new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 0.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz", NullableDecimalValue = null },
                new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 0.3562, EnumValue = MyEnum.Val2, StringValue = "ttttt", NullableDecimalValue =22 },

                //new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                //new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                //new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                //new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" },
                //new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                //new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                //new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                //new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" },
                //new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                //new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                //new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                //new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" },
                //new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                //new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                //new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                //new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" },
                //new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                //new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                //new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                //new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" }
            };

            return View(result);
        }
    }
}