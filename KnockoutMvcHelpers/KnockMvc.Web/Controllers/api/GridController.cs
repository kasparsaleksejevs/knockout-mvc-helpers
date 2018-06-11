using KnockMvc.Common;
using KnockMvc.DTO;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Routing;

namespace KnockMvc.Web.Controllers.api
{
    [TypeScriptControllerGenerate]
    [RoutePrefix("api/afolder")]
    public class MyCoolController : ApiController
    {
        // api/afolder/methodx/34/myStuffyStuff
        [Route("methodx/{id}/myStuffyStuff")]
        [HttpGet]
        public string MyStuff(int id)
        {
            return "Test" + id.ToString();
        }

        [HttpGet]
        public string MyStuff2(int id)
        {
            return "Test" + id.ToString();
        }
    }

    [TypeScriptControllerGenerate]
    public class MyCool2Controller : ApiController
    {
        // api/MyCool2/methodx/44/myStuffyStuff - does not work!
        [Route("methodx/{id}/myStuffyStuff")]
        [HttpGet]
        public string MyStuff(int id)
        {
            return "Test" + id.ToString();
        }

        //  api/MyCool2/MyStuff2?theid=3
        [HttpGet]
        public string MyStuff2(int theid)
        {
            return "Test" + theid.ToString();
        }
    }

    [TypeScriptControllerGenerate]
    public class GridController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<MyRow> Get()
        {
            var result = new List<MyRow>
            {
                new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" },

                new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" },
                new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" },
                new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" },
                new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" },
                new MyRow { IntValue = 3, BoolValue = true, DateValue = DateTime.Now, DecimalValue = 12.34m, DoubleValue = 752.234522, EnumValue = MyEnum.Val1, StringValue = "test" },
                new MyRow { IntValue = 77, BoolValue = false, DateValue = DateTime.Now.AddDays(-4).AddMinutes(-99), DecimalValue = 25, DoubleValue = 3.14, EnumValue = MyEnum.Val2, StringValue = "qwerty asdf" },
                new MyRow { IntValue = 42, BoolValue = true, DateValue = DateTime.Now.AddYears(1).ToUniversalTime(), DecimalValue = 20.445m, DoubleValue = 64.21, EnumValue = MyEnum.Val1, StringValue = "zzzzz" },
                new MyRow { IntValue = 11, BoolValue = true, DateValue = DateTime.Now.ToUniversalTime(), DecimalValue = 20.444m, DoubleValue = 3562, EnumValue = MyEnum.Val2, StringValue = "ttttt" }
            };

            return result;
        }

        // GET api/<controller>/5
        public List<string> Get(int id)
        {
            var results = new List<string>();
            results.Add(id.ToString());
            //results.Add(Url.Link("MyCoolController.MyStuff", new { id = id }));


            //var dict = new RouteValueDictionary();
            ////dict["controller"] = "MyCool";
            ////dict["action"] = "MyStuff";
            //dict["id"] = 42;
            //var path = this.GetPath("MyStuff", dict);
            //results.Add(path);

            //results.Add(GetPathWithCool());

            return results;
        }

        //public string GetPathWithCool()
        //{
        //    var url = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);

        //    return url.HttpRouteUrl2<MyCoolController>(m => m.MyStuff(34));
        //}

        //public string GetPath(string methodName, RouteValueDictionary routeValues)
        //{
        //    var configuration = System.Web.Http.GlobalConfiguration.Configuration;
        //    var apiDescription = configuration.Services.GetApiExplorer().ApiDescriptions
        //       .FirstOrDefault(c =>
        //           c.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(MyCoolController)
        //           && c.ActionDescriptor.ControllerDescriptor.ControllerType.GetMethod(methodName) != null
        //           && c.ActionDescriptor.ActionName == methodName
        //       );

        //    var route = apiDescription.Route;
        //    var routeData = new HttpRouteData(route, new HttpRouteValueDictionary(routeValues));

        //    var request = new System.Net.Http.HttpRequestMessage();
        //    request.Properties[System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey] = configuration;
        //    request.Properties[System.Web.Http.Hosting.HttpPropertyKeys.HttpRouteDataKey] = routeData;

        //    var virtualPathData = route.GetVirtualPath(request, routeValues);

        //    var path = virtualPathData.VirtualPath;

        //    return path;
        //}

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}