using Bogus;

namespace HtmlGenforStorybook.Models;

public class FooterViewModel
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public sealed class FooterFaker : Faker<FooterViewModel>
{
    public FooterFaker()
    {
        RuleFor(model => model.Title, faker => faker.Company.CompanyName());
        RuleFor(model => model.Description, faker => faker.Company.CatchPhrase());
    }
}