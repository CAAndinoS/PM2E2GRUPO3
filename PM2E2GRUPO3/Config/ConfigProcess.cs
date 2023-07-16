using System;
using System.Collections.Generic;
using System.Text;

namespace PM2E2GRUPO3.Config
{
    public static class ConfigProcess
    {
        // Web Api http
        public static String ipaddress = "192.168.1.3";
        public static string webapi = "PM2E2GRUPO3CRUD";

        //Rauting METHOD POST
        public static string postRaute = "SitCreate.php";
        //Rauting METHOD GET
        public static string getRaute = "SitioGetList.php";
        //Rauting METHOD UPDATE
        public static string updateRaute = "Update.php";
        //Rauting METHOD DELETE
        public static string deleteRaute = "Delete.php";

        //Format URL
        public static string formaturl = "http://{0}/{1}/{2}";

        // URL Endpoint
        public static string ApiGET = string.Format(formaturl, ipaddress, webapi, getRaute);
        public static string ApiPOST = string.Format(formaturl, ipaddress, webapi, postRaute);
        public static string ApiUPDATE = string.Format(formaturl, ipaddress, webapi, updateRaute);
        public static string ApiDELETE = string.Format(formaturl, ipaddress, webapi, deleteRaute);
    }
}
