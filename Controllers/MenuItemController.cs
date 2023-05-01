using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Utility;
using RedMango_API.Models.Dto;
using RedMango_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RedMango_API.Controllers
{
    [ApiController]
    [Route("api/MenuItem")]
    public class MenuItemController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IBlobService _blobService;
        private ApiResponse _response;

        public MenuItemController(ApplicationDbContext db, IBlobService blobService)
        {
            _db = db;
            _blobService = blobService;
            _response = new ApiResponse();
        }

//Route for get all menu items 
        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
            _response.Result = _db.MenuItems;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

//Route for getting 1 menu item based on id
        [HttpGet("{id:int}", Name = "GetMenuItem")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            MenuItem menuItem = _db.MenuItems.FirstOrDefault(u => u.Id == id);
            if (menuItem == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.Result = menuItem;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    
//Route to Get/Delete/Upload a new blob
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm]MenuItemCreateDTO menuItemCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemCreateDTO.File == null || menuItemCreateDTO.File.Length == 0)
                    {
                        return BadRequest();
                    }
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemCreateDTO.File.FileName)}";
                    MenuItem menuItemToCreate = new()
                    {
                        Name = menuItemCreateDTO.Name,
                        Price = menuItemCreateDTO.Price,
                        Category = menuItemCreateDTO.Category,
                        SpecialTag = menuItemCreateDTO.SpecialTag,
                        Description = menuItemCreateDTO.Description,
                        Image = await _blobService.UploadBlob(fileName, SD.SD_Storage_Container, menuItemCreateDTO.File)
                    };
                    _db.MenuItems.Add(menuItemToCreate);
                    _db.SaveChanges();

                    _response.Result = menuItemToCreate;
                    _response.StatusCode=HttpStatusCode.Created;
                    return CreatedAtRoute("GetMenuItem", new { id = menuItemToCreate.Id }, _response);
                }
                else 
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }
    
    
    }
}