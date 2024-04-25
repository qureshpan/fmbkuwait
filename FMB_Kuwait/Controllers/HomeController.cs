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
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public async Task<ActionResult> Members()
        {
            MemberDataViewModel model = new MemberDataViewModel();
            model.memberDatasList = new List<MemberData>();
            DataSet ds = await DB.ExecuteStoredProcDataSetAsync("sp_Get_MemberList", null);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    model.memberDatasList.Add(new MemberData
                    {
                        Id = dr["Id"].ToString(),
                        Count = Parser.ParseInt(dr["TotalTags"].ToString()),
                        Floor = dr["Floor"].ToString(),
                        Flat = dr["Flat"].ToString(),
                        HouseId = dr["HouseId"].ToString(),
                        DeliveryPerson = dr["DeliveryPerson"].ToString(),
                        DeliveryPerson2 = dr["DeliveryPerson2"].ToString(),
                        MasoolName = dr["MasoolName"].ToString(),
                        Mobile = dr["Mobile"].ToString(),
                        ThaliNumber = dr["ThaliNumber"].ToString(),
                        ThaliCode = dr["ThaliCode"].ToString(),
                        Size = Parser.ParseInt(dr["Size"].ToString()),
                        NewCode = dr["NewCode"].ToString(),
                    });


                }
            }
            return View(model);

        }
        public async Task<ActionResult> AddTag(int Id)
        {
            Session["MemberId"] = Id;
            TagDetail model = new TagDetail();
            model.tags = new List<Tag>();
            model.MemberId = Id;
            model.tags = await GetTags(Id);
            return View(model);
        }

        private async Task<List<Tag>> GetTags(int id)
        {
            List<Tag> tags = new List<Tag>();
            SqlParameter[] spa ={
                                     new SqlParameter(){
                                        ParameterName="@CustId",
                                        SqlDbType=SqlDbType.Int,
                                        Value=id
                                    }
                               };
            DataSet ds = await DB.ExecuteStoredProcDataSetAsync("sp_GetTagsByCustId", spa);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    tags.Add(new Tag { TagName = dr["TagID"].ToString() });
                }
            }
            return tags;
        }

        [HttpPost]
        public async Task<ActionResult> AddTag(TagDetail model)
        {
            SqlParameter[] spa ={
                                     new SqlParameter(){
                                        ParameterName="@custid",
                                        SqlDbType=SqlDbType.Int,
                                        Value=model.MemberId
                                    },
                                      new SqlParameter(){
                                        ParameterName="@tagid",
                                        SqlDbType=SqlDbType.NVarChar,
                                        Value=model.tag.TagName
                                    }
                               };
            try
            {
                DataSet ds = await DB.ExecuteStoredProcDataSetAsync("sp_AddTag", spa);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    model.IsSuccess = Parser.ParseBoolean(dr["IsSuccess"].ToString());
                    model.Message = dr["Message"].ToString();
                }
            }
            catch
            {

            }
            ModelState.Clear();
            model.tags = await GetTags(model.MemberId);

            return View(model);
        }
        public async Task<ActionResult> Tags(int Id)
        {
            SqlParameter[] spa ={
                                     new SqlParameter(){
                                        ParameterName="@CustId",
                                        SqlDbType=SqlDbType.Int,
                                        Value=Id
                                    }
                               };
            DataSet ds = await DB.ExecuteStoredProcDataSetAsync("sp_GetTagsByCustId", spa);
            List<string> tags = new List<string>();

            return View(tags);
        }
        public async Task<ActionResult> DeleteTag(string Id)
        {
            TagDetail model = new TagDetail();
            model.tags = new List<Tag>();
            model.MemberId = Parser.ParseInt(Session["MemberId"].ToString());
            SqlParameter[] spa ={
                                     new SqlParameter(){
                                        ParameterName="@tagId",
                                        SqlDbType=SqlDbType.NVarChar,
                                        Value=Id
                                    }
                               };
            DataSet ds = await DB.ExecuteStoredProcDataSetAsync("sp_Delete_Tag", spa);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                model.IsSuccess = Parser.ParseBoolean(dr["IsSuccess"].ToString());
                model.Message = dr["Message"].ToString();
            }
            model.tags = await GetTags(model.MemberId);
            return View("AddTag", model);
        }
    }
}