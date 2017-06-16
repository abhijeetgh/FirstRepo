using EZRAC.Risk.UI.Web.Attributes;
using EZRAC.Risk.UI.Web.Helper;
using EZRAC.Risk.UI.Web.ViewModels.Claims;
using EZRAC.RISK.Services.Contracts;
using EZRAC.RISK.Services.Contracts.Dtos;
using EZRAC.RISK.Util.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EZRAC.Risk.UI.Web.Controllers
{
    
    public class RiskFilesController : Controller
    {
        IRiskFileService _riskFileService;
        

        public RiskFilesController(IRiskFileService riskFileService )
        {

            _riskFileService = riskFileService;
         
        }


        [UserActionLog(UserAction = UserActionLogConstant.ViewFiles)]
        public async Task<ActionResult> Index(int claimNumber)
        {            
            ViewBag.FileList = LookUpHelpers.GetFileCategoryListItems();
            ViewBag.ClaimId = claimNumber;
            return View();
        }
        
        public async Task<ActionResult> ListFiles(long id)
        {
            var fileList = await _riskFileService.GetFilesByClaimIdAsync(id);

            List<RiskFileModel> files = new List<RiskFileModel>();
            var groupByResult = fileList.GroupBy(x => x.CategoryName);

            foreach (var item in groupByResult)
            {
                files.Add(
                    new RiskFileModel
                    {
                        CategoryName = item.Key,
                        CategoryId= item.FirstOrDefault().CategoryId,
                        FileList = item.Select(x => new FileModel { Id = x.FileId, FileName = x.FileName}).ToList()
                    });

            }
            return View(files);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [CRMSAuthroize(ClaimType = ClaimsConstant.AddFiles)]
        [UserActionLog(UserAction = UserActionLogConstant.AddFile)]
        public async Task<JsonResult> Post()
        {
            try
            {
                await SaveFile();
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }


            return Json("File uploaded successfully");

        }


        [AcceptVerbs(HttpVerbs.Delete)]
        [CRMSAuthroize(ClaimType=ClaimsConstant.DeleteFiles)]
        [UserActionLog(UserAction = UserActionLogConstant.DeletteGroupFiles)]
        public async Task<JsonResult> DeleteCategory(int id, long claimId)
        {

            var fileList = await _riskFileService.GetFilesByClaimIdAsync(claimId);

            foreach (var item in fileList)
            {
                var path = Server.MapPath("~/App_Data/Images/Claims/");

                var oldImagePath = Path.Combine(path, item.FilePath);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                await _riskFileService.DeleteFilesByCategoryAsync(claimId, id);
            }
            return Json("File deleted!!!");
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [CRMSAuthroize(ClaimType = ClaimsConstant.DeleteFiles)]
        [UserActionLog(UserAction = UserActionLogConstant.DeleteFile)]
        public async Task<JsonResult> Delete(int id)
        {
            var fileDto = await _riskFileService.GetFileAsync(id);

            if (fileDto != null)
            {
                var path = Server.MapPath("~/App_Data/Images/Claims/");

                var oldImagePath = Path.Combine(path, fileDto.FilePath);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                await _riskFileService.DeleteFileAsync(id);
            }
            return Json("File deleted!!!");
        }

        [UserActionLog(UserAction = UserActionLogConstant.DownLoadFile)]
        public async Task<ActionResult> Download(int id)
        {
            var fileDto = await _riskFileService.GetFileAsync(id);

            string filePath = Server.MapPath("~/App_Data/Images/Claims/");

            string displayName = fileDto.FileName;

            if (System.IO.File.Exists(Path.Combine(filePath, fileDto.FilePath)))
            {
                string contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                return File(Path.Combine(filePath, fileDto.FilePath), contentType, displayName);
            }

            return null;
        }

        #region private method
        private async Task<bool> SaveFile()
        {
            var success = false;
            string claimId = Request.Form["ClaimID"];
            string fileCategoryId = Request.Form["SelectedFileTypeId"];
            foreach (string file in Request.Files)
            {

                var fileContent = Request.Files[file];
                var filePath = string.Empty;

                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var orginalFileName = Path.GetFileName(fileContent.FileName);
                    var fileName = Path.GetFileName(fileContent.FileName);

                    var path = Server.MapPath("~/App_Data/Images/Claims/") + claimId;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if (Directory.Exists(path))
                    {
                        string[] filestringarray = fileName.Split('.');
                        if (filestringarray.Length > 1)
                        {
                            filestringarray[filestringarray.Length - 2] = filestringarray[filestringarray.Length - 2] + System.Guid.NewGuid().ToString();
                            fileName = string.Join(".", filestringarray);
                        }
                    }
                    filePath = claimId + "/" + fileName;
                    path = Path.Combine(path, fileName);
                    fileContent.SaveAs(path);

                    PicturesAndFilesDto fileDto = new PicturesAndFilesDto
                    {
                        ClaimId = claimId,
                        RiskFilesCategory = (RiskFilesCategory)Enum.Parse(typeof(RiskFilesCategory), fileCategoryId),
                        FileName = orginalFileName,
                        FilePath = filePath,
                    };

                    await _riskFileService.SaveFileAsync(fileDto);
                    success = true;
                }
            }

            return success;
        }
        #endregion
    }
}