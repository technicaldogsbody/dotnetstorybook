using Bogus;

namespace HtmlGenforStorybook.Models;

public class HeaderViewModel
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public sealed class HeaderFaker : Faker<HeaderViewModel>
{
    public HeaderFaker()
    {
        RuleFor(model => model.Title, faker => faker.Company.CompanyName());
        RuleFor(model => model.Description, faker => faker.Company.Bs());
    }
}