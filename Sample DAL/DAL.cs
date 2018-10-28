using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DALLibrary;
using System.Drawing;

namespace Sample_DAL {
    class DAL {

        private static string ServerIp { get; set; }
        private static string DatabaseName { get; set; }
        private static string UserId { get; set; }
        private static string Password { get; set; } //Not a good idea to save password here, just for practice

        private static string ReadOnlyConnectionString = string.Format("\"Server={0};Database={1};User Id={2};Password={3};",ServerIp,DatabaseName,UserId,Password);
        private static string EditOnlyConnectionString = string.Format("\"Server={0};Database={1};User Id={2};Password={3};", ServerIp, DatabaseName, UserId, Password);

        public DAL(string server, string database, string id, string password) {
            ServerIp = server;
            DatabaseName = database;
            UserId = id;
            Password = password;
        }

        #region Photo

        /// <summary>
        /// Gets a list of all Photo objects from the database.
        /// </summary>
        /// <remarks></remarks>
        public static List<Photo> PhotoGetAll() {
            SqlCommand comm = new SqlCommand();
            List<Photo> retList = new List<Photo>();
            try {
                comm.CommandText = "sprocPhotosGetAll";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Connection = new SqlConnection(ReadOnlyConnectionString);
                comm.Connection.Open();
                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read()) {
                    Photo newObj = FillPhoto(dr);
                    retList.Add(newObj);
                }
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            } finally {
                if (comm.Connection != null)
                    comm.Connection.Close();
            }
            return retList;
        }

        /// <summary>
        /// Gets the Photo for the given ID
        /// </summary>
        /// <remarks></remarks>

        public static Photo PhotoGetByID(int id) {
            SqlCommand comm = new SqlCommand();
            Photo retObj = null;
            try {
                comm.CommandText = "sprocPhotoGetByID";
                comm.CommandType = System.Data.CommandType.StoredProcedure;

                comm.Parameters.AddWithValue("@PhotoID", id);
                comm.Connection = new SqlConnection(ReadOnlyConnectionString);
                comm.Connection.Open();

                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read()) {
                    retObj = FillPhoto(dr);
                }
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            } finally {
                if (comm.Connection != null)
                    comm.Connection.Close();
            }
            return retObj;
        }

        /// <summary>
        /// Attempts to add a Photo to the database
        /// </summary>
        /// <remarks></remarks>

        public static int PhotoAdd(Photo photo) {
            int retInt = -1;
            if (photo == null) return -1;

            SqlCommand comm = new SqlCommand();
            try {
                comm.CommandText = "sproc_PhotoAdd";
                comm.Parameters.AddWithValue("@PhotographerID", photo.PhotographerID);
                comm.Parameters.AddWithValue("@LocationID", photo.LocationID);
                comm.Parameters.AddWithValue("@Subject", photo.Subject);
                comm.Parameters.AddWithValue("@Image", photo.PhotoImage);
                comm.Parameters.AddWithValue("@Date", photo.Date);

                SqlParameter retParameter;
                retParameter = new SqlParameter("@PhotographerID", System.Data.SqlDbType.Int);
                retParameter.Direction = System.Data.ParameterDirection.Output;
                comm.Parameters.Add(retParameter);

                comm.Connection = new SqlConnection(EditOnlyConnectionString);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Connection.Open();

                int rowsAffected = comm.ExecuteNonQuery();
                if (rowsAffected != 1) {
                    //There was a problem
                    retInt = -1;
                }
                else
                    retInt = (int)retParameter.Value;
                photo.PhotoID = retInt;
            } catch (Exception ex) {
                retInt = -1;
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            } finally {
                if (comm.Connection != null)
                    comm.Connection.Close();
            }
            return retInt;
        }

        /// <summary>
        /// Attempts to update a Photo in the database
        /// </summary>
        /// <remarks></remarks>

        public static int PhotoUpdateByID(Photo photo) {
            int retInt;
            if (photo == null) return -1;
            SqlCommand comm = new SqlCommand();
            try {
                comm.CommandText = "sproc_PhotoUpdateByID";
                comm.Parameters.AddWithValue("@PhotoID", photo.PhotoID);
                comm.Parameters.AddWithValue("@PhotographerID", photo.PhotographerID);
                comm.Parameters.AddWithValue("@LocationID", photo.LocationID);
                comm.Parameters.AddWithValue("@Subject", photo.Subject);
                comm.Parameters.AddWithValue("@Image", photo.PhotoImage);
                comm.Parameters.AddWithValue("@Date", photo.Date);

                comm.Connection = new SqlConnection(EditOnlyConnectionString);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Connection.Open();

                retInt = comm.ExecuteNonQuery();
            } catch (Exception ex) {
                retInt = -1;
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            } finally {
                if (comm.Connection != null)
                    comm.Connection.Close();
            }
            return retInt;
        }



        /// <summary>
        /// Fill in a photo object with information from the DataReader
        /// </summary>
        /// <param name="dr"></param>
        /// <returns>a filled-in employee object</returns>
        private static Photo FillPhoto(SqlDataReader dr) {
            Photo newObj = new Photo();
            newObj.PhotoID = (int)dr["PhotoID"];
            newObj.PhotographerID = (int)dr["PhotographerID"];
            newObj.LocationID = (int)dr["LocationID"];
            newObj.Subject = (string)dr["Subject"];
            newObj.PhotoImage = (Image)dr["PhotoImage"];
            return newObj;

        }

        #endregion Photo

        #region Photographer
        /// <summary>
        /// Given a Last and First name, determine if photographer is in the database
        /// </summary>
        /// <remarks>Returns the 1 if photographer is in the database
        /// Returns -1 if not it database
        /// Returns -3 if Error</remarks>

        public static int PhotographerInDatabase(string LastName, string FirstName) {
            int retInt = -1;
            if (LastName == null && FirstName == null)
                return -1;

            SqlCommand comm = new SqlCommand();

            try {
                comm.CommandText = "sprocPhotographerInDatabase";
                comm.Parameters.AddWithValue("@LastName", LastName);
                comm.Parameters.AddWithValue("@FirstName", FirstName);

                SqlParameter retParameter;
                retParameter = new SqlParameter("@Return", System.Data.SqlDbType.Int);
                retParameter.Direction = System.Data.ParameterDirection.ReturnValue;
                comm.Parameters.Add(retParameter);

                comm.Connection = new SqlConnection(ReadOnlyConnectionString);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Connection.Open();

                comm.ExecuteNonQuery();
                retInt = (int)retParameter.Value;


            } catch (Exception ex) {
                retInt = -3;
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            } finally {
                if (comm.Connection != null)
                    comm.Connection.Close();
            }
            return retInt;
        }

        /// <summary>
        /// Gets a list of all Photographers whose first or last name contains a string.
        /// </summary>
        /// <remarks></remarks>
        public static List<Photographer> PhotographersGetByString(string Str) {
            if (Str == null)
                return null;


            SqlCommand comm = new SqlCommand();
            List<Photographer> retList = new List<Photographer>();
            try {
                comm.CommandText = "sprocPhotographersGetByString";
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Connection = new SqlConnection(ReadOnlyConnectionString);
                comm.Parameters.AddWithValue("@String", Str);
                comm.Connection.Open();

                SqlDataReader dr = comm.ExecuteReader();
                while (dr.Read()) {
                    Photographer newObj = FillPhotographer(dr);
                    retList.Add(newObj);
                }
            } catch (Exception ex) {
                retList = null;
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            } finally {
                if (comm.Connection != null)
                    comm.Connection.Close();
            }
            return retList;
        }

        /// <summary>
        /// Fill in a photographer object with information from the DataReader
        /// </summary>
        /// <param name="dr"></param>
        /// <returns>a filled-in employee object</returns>
        private static Photographer FillPhotographer(SqlDataReader dr) {
            Photographer newObj = new Photographer();
            newObj.PhotographerID = (int)dr["PhotographerID"];
            newObj.FirstName = (string)dr["FirstName"];
            newObj.LastName = (string)dr["LastName"];
            newObj.Email = (string)dr["Email"];
            return newObj;

        }
        #endregion Photographer

    }
}
