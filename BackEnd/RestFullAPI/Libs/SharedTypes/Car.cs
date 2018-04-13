using System;
using JsonMaker;

namespace RestFullAPI.Libs.SharedTypes
{
    public class Car
    {
        public string id = "";
        public string veichle = "";
        public string vendor = "";
        public int year = 0;
        public string description = "";
        public bool sold = false;
        public DateTime createdAt;
        public DateTime updatedAt;

        public string ToJson()
        {
            JSON jm = new JSON();
            jm.setString("id", id);
            jm.setString("veichle", veichle);
            jm.setString("vendor", vendor);
            jm.setInt("year", year);
            jm.setString("description", description);
            jm.setBoolean("sold", sold);
            jm.setDateTime("createdAt", createdAt);
            jm.setDateTime("updatedAt", updatedAt);

            return jm.ToJson();

        }


    }
}
