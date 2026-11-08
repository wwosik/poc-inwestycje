using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortfelInwestycji.Server.Assets;

namespace PortfelInwestycji.Server.Pages.Assets;

public class IndexModel(AssetService assetService) : PageModel
{
    [BindProperty]
    public AssetInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public IReadOnlyList<Asset> Assets { get; private set; } = [];

    public IReadOnlyList<CurrencyTotal> Totals { get; private set; } = [];

    public IEnumerable<SelectListItem> KindOptions =>
        Enum.GetValues<AssetKind>().Select(kind => new SelectListItem(kind.Label(), kind.ToString()));

    public IEnumerable<SelectListItem> CurrencyOptions =>
        Currencies.Supported.Select(code => new SelectListItem(code, code));

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        var asset = await assetService.AddAsync(Input, cancellationToken);
        StatusMessage = $"Dodano aktywo {asset.Name}.";
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Assets = await assetService.ListAsync(cancellationToken);
        Totals = AssetService.TotalsByCurrency(Assets);
    }
}
