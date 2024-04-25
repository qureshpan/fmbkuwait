using FMB_Kuwait.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FMB_Kuwait.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(string username,string password)
        {
            SqlParameter[] spa ={
                                     new SqlParameter(){
                                        ParameterName="@UserId",
                                        SqlDbType=SqlDbType.NVarChar,
                                        Value=username
                                    },
                                     new SqlParameter(){
                                        ParameterName="@Password",
                                        SqlDbType=SqlDbType.NVarChar,
                                        Value=password
                                    }
                               };
            DataSet ds = await DB.ExecuteStoredProcDataSetAsync("sp_GetLoginDetails", spa);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Session["UserName"] = ds.Tables[0].Rows[0]["UserName"].ToString();
                Session["LoginName"] = ds.Tables[0].Rows[0]["LoginName"].ToString();
                Session["id"] = ds.Tables[0].Rows[0]["Id"].ToString();
                return RedirectToAction("Members", "Home");
            }
            else
            {
                ViewBag.Message = "ds is null";
                return View();
            }
        }
    }
}