using GarageCustomerAdmin._Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class locationDB : baseDB
    {
        public static LocationBLL repo;
        public static DataTable _dt;
        public static DataSet _ds;

        public LocationTimings[] LocationTimings { get; private set; }

        public locationDB()
           : base()
        {
            repo = new LocationBLL();
            _dt = new DataTable();
            _ds = new DataSet();
        }

        public List<LocationBLL> GetAll()
        {
            try
            {
                var lst = new List<LocationBLL>();
                SqlParameter[] p = new SqlParameter[0];
                _dt = (new DBHelperGarageUAT().GetTableFromSP)("sp_getLocation_CADMIN", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<LocationBLL>>();
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<string> GetLocationImages(int id)
        {

            try
            {
                var _obj = new LocationBLL();
                List<string> ImagesSource = new List<string>();
                _dt = new DataTable();
                SqlParameter[] p1 = new SqlParameter[1];
                p1[0] = new SqlParameter("@id", id);
                _dt = (new DBHelperGarageUAT().GetTableFromSP)("sp_GetLocationImages_CAdmin", p1);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        _obj.LocationImages = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<LocationimagesBLL>>();

                        for (int i = 0; i < _obj.LocationImages.Count; i++)
                        {
                            ImagesSource.Add(_obj.LocationImages[i].ImageURL);
                        }
                    }
                }

                return ImagesSource;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<LocationTimings> GetTimings(int id)
        {

            try
            {
                var _obj = new LocationBLL();
                _dt = new DataTable();
                SqlParameter[] p1 = new SqlParameter[1];
                p1[0] = new SqlParameter("@id", id);
                _dt = (new DBHelperGarageUAT().GetTableFromSP)("sp_GetLocationTiming_CAdmin", p1);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        return JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<LocationTimings>>();
                    }
                }
                return new List<LocationTimings>();

            }
            catch (Exception ex)
            {
                return new List<LocationTimings>();
            }
        }
        public LocationBLL Get(int id)
        {
            try
            {
                var _obj = new LocationBLL();
                string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
                string[] arabicdays = { "الأحد", "الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة", "السبت" };
                // Create a dictionary to store day name as key and index as value
                Dictionary<string, string> dayDictionary = new Dictionary<string, string>();

                // Populate the dictionary
                for (int i = 0; i < days.Length; i++)
                {
                    dayDictionary.Add(days[i], arabicdays[i]);
                }
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@id", id);
                _dt = (new DBHelperGarageUAT().GetTableFromSP)("sp_GetLocationsByID_CADMIN", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        _obj = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<LocationBLL>>().FirstOrDefault();
                        _obj.LocationTimings = GetTimings(_obj.LocationID);
                        if (_obj.LocationTimings.Count == 0)
                        {
                            foreach (var item in dayDictionary)
                            {
                                _obj.LocationTimings.Add(new LocationTimings
                                {
                                    Name = item.Key,
                                    Time = "",
                                    AName = item.Value,
                                    ATime = "",
                                    LocationID = id
                                });
                            }
                        }
                    }
                }
                return _obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public int Insert(LocationBLL data)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[24];

                p[0] = new SqlParameter("@Name", data.Name);
                p[0] = new SqlParameter("@ArabicName", data.ArabicName);
                p[1] = new SqlParameter("@Description", data.Descripiton);
                p[1] = new SqlParameter("@ArabicDescription", data.ArabicDescription);
                p[2] = new SqlParameter("@Address", data.Address);
                p[2] = new SqlParameter("@ArabicAddress", data.ArabicAddress);
                p[3] = new SqlParameter("@ContactNo", data.ContactNo);
                p[4] = new SqlParameter("@Email", data.Email);
                //p[5] = new SqlParameter("@TimeZoneID", data.TimeZoneID);
                //p[6] = new SqlParameter("@CountryID", data.CountryID);
                //p[7] = new SqlParameter("@LicenseID", data.LicenseID);
                //p[8] = new SqlParameter("@CityID", data.CityID);
                //p[9] = new SqlParameter("@UserID", data.UserID);                
                //p[10] = new SqlParameter("@Longitude", data.Longitude);
                //p[11] = new SqlParameter("@Latitude", data.Latitude);
                //p[12] = new SqlParameter("@DeliveryServices", data.DeliveryServices);
                //p[13] = new SqlParameter("@DeliveryCharges", data.DeliveryCharges);
                //p[14] = new SqlParameter("@DeliveryTime", data.DeliveryTime);
                //p[15] = new SqlParameter("@MinOrderAmount", data.MinOrderAmount);               
                p[16] = new SqlParameter("@LastUpdatedBy", "Admin");
                p[17] = new SqlParameter("@LastUpdatedDate", data.LastUpdatedDate);
                p[18] = new SqlParameter("@StatusID", data.StatusID);
                //p[19] = new SqlParameter("@CompanyCode", data.CompanyCode);
                p[20] = new SqlParameter("@ImageURL", data.ImageURL);
                //p[21] = new SqlParameter("@Opentime", data.Open_Time);
                //p[22] = new SqlParameter("@Closetime", data.Close_Time);
                p[23] = new SqlParameter("@LocationID", data.LocationID);

                rtn = (new DBHelperGarageUAT().ExecuteNonQueryReturn)("dbo.sp_InsertLocation_CADMIN", p);

                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int Update(LocationBLL data)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[22];
                p[0] = new SqlParameter("@Name", data.Name);
                p[1] = new SqlParameter("@Descripiton", data.Descripiton);
                p[2] = new SqlParameter("@Address", data.Address);
                p[3] = new SqlParameter("@ContactNo", data.ContactNo);
                p[4] = new SqlParameter("@Email", data.Email);
                p[5] = new SqlParameter("@Longitude", data.Longitude);
                p[6] = new SqlParameter("@Latitude", data.Latitude);
                p[7] = new SqlParameter("@LastUpdatedBy", "Admin");
                p[8] = new SqlParameter("@LandmarkID", data.LandmarkID);
                p[9] = new SqlParameter("@StatusID", data.StatusID);
                p[10] = new SqlParameter("@IsFeatured", data.IsFeatured);
                p[11] = new SqlParameter("@LocationID", data.LocationID);
                p[12] = new SqlParameter("@GMapLink", data.Gmaplink);
                p[13] = new SqlParameter("@ArabicName", data.ArabicName);
                p[14] = new SqlParameter("@ArabicDescription", data.ArabicDescription);
                p[15] = new SqlParameter("@ArabicAddress", data.ArabicAddress);
                p[16] = new SqlParameter("@CustomerStatusID", data.CustomerStatusID);
                p[17] = new SqlParameter("@BrandImage", data.BrandThumbnailImage);
                p[18] = new SqlParameter("@CountryCode", data.CountryID);
                p[19] = new SqlParameter("@CityID", data.CityID);
                p[20] = new SqlParameter("@UserID", data.UserID);
                p[21] = new SqlParameter("@BusinessType", data.BusinessType);


                rtn = (new DBHelperGarageUAT().ExecuteNonQueryReturn)("dbo.sp_UpdateLocation_CADMIN", p);

                if (data.Amenities != "")
                {
                    SqlParameter[] p1 = new SqlParameter[3];

                    p1[0] = new SqlParameter("@Amenities", data.Amenities == "" ? null : data.Amenities);
                    p1[1] = new SqlParameter("@LocationID", data.LocationID);
                    p1[2] = new SqlParameter("@LastUpdatedDate", DateTime.Now);
                    (new DBHelperGarageUAT().ExecuteNonQueryReturn)("sp_insertLocationAmenities_CAdmin", p1);
                }
                if (data.Service != "")
                {
                    SqlParameter[] p2 = new SqlParameter[3];

                    p2[0] = new SqlParameter("@Service", data.Service == "" ? null : data.Service);
                    p2[1] = new SqlParameter("@LocationID", data.LocationID);
                    p2[2] = new SqlParameter("@LastUpdatedDate", DateTime.Now);
                    (new DBHelperGarageUAT().ExecuteNonQueryReturn)("sp_insertLocationServices_CAdmin", p2);
                }
                try
                {
                    var imgStr = String.Join(",", data.LocationImages.Select(p => p.ImageURL));
                    SqlParameter[] p3 = new SqlParameter[3];
                    p3[0] = new SqlParameter("@Images", imgStr);
                    p3[1] = new SqlParameter("@LocationID", data.LocationID);
                    p3[2] = new SqlParameter("@LastUpdatedDate", DateTime.Now);
                    (new DBHelperGarageUAT().ExecuteNonQueryReturn)("sp_insertLocationImages_CAdmin", p3);
                }
                catch { }
                try
                {
                    SqlParameter[] d = new SqlParameter[1];
                    d[0] = new SqlParameter("@id", data.LocationID);
                    (new DBHelper().ExecuteNonQueryReturn)("sp_deleteLocationTiming_CADMIN", d);

                    if (data.LocationTimings[0].Time != null)
                    {
                        foreach (var timings in data.LocationTimings)
                        {
                            SqlParameter[] p4 = new SqlParameter[5];
                            p4[0] = new SqlParameter("@LocationID", data.LocationID);
                            p4[1] = new SqlParameter("@Name", timings.Name);
                            p4[2] = new SqlParameter("@Time", timings.Time);
                            p4[3] = new SqlParameter("@ArabicName", timings.AName);
                            p4[4] = new SqlParameter("@ArabicTime", timings.ATime);
                            (new DBHelper().ExecuteNonQueryReturn)("sp_InsertLocationTimings_CADMIN", p4);
                        }
                    }
                }
                catch { }

                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int Delete(LocationBLL data)
        {
            try
            {
                int _obj = 0;
                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@id", data.LocationID);
                p[1] = new SqlParameter("@LastUpdatedDate", data.LastUpdatedDate);

                _obj = (new DBHelperGarageUAT().ExecuteNonQueryReturn)("sp_DeleteLocation_CADMIN", p);

                return _obj;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
