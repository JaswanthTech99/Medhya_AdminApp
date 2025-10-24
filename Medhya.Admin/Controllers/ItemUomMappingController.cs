using Medhya.Admin.Models;
using Medhya.Admin.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
public class ItemUomMappingController : Controller
{
    private readonly IItemRepository _repo;
    private readonly ILogger<ItemUomMappingController> _logger;

    public ItemUomMappingController(IItemRepository repo, ILogger<ItemUomMappingController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    // GET: /ItemUomMapping/Index?itemId=1
    [HttpGet]
    public async Task<IActionResult> Index(int? itemId)
    {
        // TODO: replace with your real item list for dropdown
        // e.g., await _repo.GetAllAsync() or a light list with (Id, ItemName)
        var items = await _repo.GetItemList(); // you already have this
        ViewBag.ItemList = items
            .Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.ItemName })
            .OrderBy(x => x.Text)
            .ToList();

        var chosenId = itemId ?? items.FirstOrDefault()?.Id ?? 0;
        ViewBag.SelectedItemId = chosenId;

        var grid = chosenId > 0
            ? await _repo.GetItemUomGridAsync(chosenId)
            : Enumerable.Empty<ItemUomAllowedRow>();

        return View(grid);
    }

    // AJAX: POST /ItemUomMapping/Set
    [HttpPost]
    public async Task<IActionResult> Set(int itemId, string uom, bool allow)
    {
        try
        {
            var user = User?.Identity?.Name;
            await _repo.SetItemUomAllowedAsync(itemId, uom, allow, user);
            return Ok(new { ok = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set mapping for Item {ItemId}, UOM {UOM}", itemId, uom);
            return StatusCode(500, new { ok = false, message = "Failed to save mapping" });
        }
    }

    // AJAX: GET /ItemUomMapping/GetGrid?itemId=1
    [HttpGet]
    public async Task<IActionResult> GetGrid(int itemId)
    {
        var rows = await _repo.GetItemUomGridAsync(itemId);
        return PartialView("_ItemUomGrid", rows); // returns just the table
    }
}
