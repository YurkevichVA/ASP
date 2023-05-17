using ASP_201.Data;
using ASP_201.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_201.Controllers
{
    [Route("api/rates")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public RatesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public object Get([FromQuery] string data)
        {
            return new { result = $"Запит оброблено методом GET і прийнято дані {data}" };
        }
        [HttpPost]
        public object Post([FromBody] BodyData bodyData)
        {
            int statusCode;
            string result;

            if (bodyData == null || bodyData.Data == null || bodyData.ItemId == null || bodyData.UserId == null)
            {
                statusCode = StatusCodes.Status400BadRequest;
                result = $"Не всі дані передані: Data={bodyData?.Data} ItemId={bodyData?.ItemId} UserId={bodyData?.UserId}";
            }
            else
            {
                try
                {
                    Guid itemId = Guid.Parse(bodyData.ItemId);
                    Guid userId = Guid.Parse(bodyData.UserId);
                    int rating = Convert.ToInt32(bodyData.Data);

                    Rate? rate = _dataContext.Rates.FirstOrDefault(r => r.UserID == userId && r.ItemId == itemId);
                    if (rate is not null)
                    {
                        if(rate.Rating == rating)
                        {
                            statusCode = StatusCodes.Status406NotAcceptable;
                            result = $"Дані вже наявні: ItemId={bodyData.ItemId} UserId={bodyData.UserId}";
                        }
                        else
                        {
                            rate.Rating = rating;
                            _dataContext.SaveChanges();
                            statusCode = StatusCodes.Status202Accepted;
                            result = $"Дані оновлено: ItemId={bodyData.ItemId} UserId={bodyData.UserId}";
                        }
                        
                    }
                    else
                    {
                        _dataContext.Rates.Add(new()
                        {
                            ItemId = itemId,
                            UserID = userId,
                            Rating = rating
                        });
                        _dataContext.SaveChanges();
                        statusCode = StatusCodes.Status201Created;
                        result = $"Дані внесено: ItemId={bodyData.ItemId} UserId={bodyData.UserId}";
                    }

                }
                catch
                {
                    statusCode = StatusCodes.Status400BadRequest;
                    result = $"Дані не опрацьовані: Data={bodyData?.Data} ItemId={bodyData.ItemId} UserId={bodyData.UserId}";
                }
            }
            HttpContext.Response.StatusCode = statusCode;

            return new {result};
        }
        public object Default()
        {
            switch(HttpContext.Request.Method)
            {
                case "LINK": return Link();
                case "UNLINK": return Unlink();
                case "PATCH": return Patch();
                default: throw new NotImplementedException();
            }
        }
        private object Link()
        {
            return new { result = $"Запит оброблено методом LINK і прийнято дані --" };
        }
        private object Patch()
        {
            return new { result = $"Запит оброблено методом PATCH і прийнято дані --" };
        }
        private object Unlink()
        {
            return new { result = $"Запит оброблено методом UNLINK і прийнято дані --" };
        }
        [HttpDelete]
        public object Delete([FromBody] BodyData bodyData)
        {
            int statusCode;
            string result;

            if (bodyData == null || bodyData.Data == null || bodyData.ItemId == null || bodyData.UserId == null)
            {
                statusCode = StatusCodes.Status400BadRequest;
                result = $"Не всі дані передані: Data={bodyData?.Data} ItemId={bodyData?.ItemId} UserId={bodyData?.UserId}";
            }
            else
            {
                try
                {
                    Guid itemId = Guid.Parse(bodyData.ItemId);
                    Guid userId = Guid.Parse(bodyData.UserId);
                    int rating = Convert.ToInt32(bodyData.Data);

                    Rate? rate = _dataContext.Rates.FirstOrDefault(r => r.UserID == userId && r.ItemId == itemId);
                    if (rate is not null)
                    {
                        _dataContext.Rates.Remove(rate);
                        _dataContext.SaveChanges();
                        statusCode = StatusCodes.Status202Accepted;
                        result = $"Дані видалено: ItemId={bodyData.ItemId} UserId={bodyData.UserId}";
                    }
                    else
                    {
                        statusCode = StatusCodes.Status406NotAcceptable;
                        result = $"Дані відсутні (не можуть бути видалені): ItemId={bodyData.ItemId} UserId={bodyData.UserId}";
                    }

                }
                catch
                {
                    statusCode = StatusCodes.Status400BadRequest;
                    result = $"Дані не опрацьовані: Data={bodyData?.Data} ItemId={bodyData.ItemId} UserId={bodyData.UserId}";
                }
            }
            HttpContext.Response.StatusCode = statusCode;

            return new { result };
        }
    }
    public class BodyData
    {
        public string? Data { get; set; }
        public string? ItemId { get; set; }
        public string? UserId { get; set; }
    }
}
