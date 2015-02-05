using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;
using System.IO;

namespace Device_Programmer
{
  public class ProgrammerController : ApiController
  {
    // GET api/<controller>/5
    public async Task<HttpResponseMessage> Get(string currentIp)
    {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://" + currentIp);
                using (HttpResponseMessage response = await client.GetAsync("tvdevicedesc.xml"))
                {
                    using (HttpContent content = response.Content)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(root));
                        Stream reader = await content.ReadAsStreamAsync();
                        try
                        {
                            root deviceDescriptionRoot = (root)serializer.Deserialize(reader);
                            return Request.CreateResponse(HttpStatusCode.OK, deviceDescriptionRoot);
                        }
                        catch (Exception ex)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
                        }
                    }
                }
            }
    }

    // POST api/<controller>
    public async Task<HttpResponseMessage> Post([FromBody]ProgrammingSettings settings)
    {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://" + settings.CurrentIpAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringBuilder sb = new StringBuilder();
                sb.Append("/cgi-bin/http.cgi?network:set&");
                sb.Append(settings.EnableDHCP.ToString());
                sb.Append('&');
                sb.Append(settings.DeviceProgrammedIP);
                sb.Append('&');
                sb.Append(settings.Gateway);
                sb.Append('&');
                sb.Append(settings.NetMask);
                sb.Append('&');
                sb.Append(settings.DNS);

                HttpResponseMessage response = await client.GetAsync(sb.ToString());
                if (response.IsSuccessStatusCode)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Device Programmed Successfully.");
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to Program Sensor.");
            }
    }

    // PUT api/<controller>/5
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/<controller>/5
    public void Delete(int id)
    {
    }
  }

    public class ProgrammingSettings
    {
        public string DeviceName { get; set; }
        public string CurrentIpAddress { get; set; }
        public bool EnableDHCP { get; set; }
        public string DeviceProgrammedIP { get; set; }
        public string Gateway { get; set; }
        public string NetMask { get; set; }
        public string DNS { get; set; }
        public string ProgrammedBy { get; set; }
    }
}