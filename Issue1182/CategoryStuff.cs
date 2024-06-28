namespace Issue1182;

public class CategoryGroupAttribute : CategoryAttribute
{
    public CategoryGroupAttribute(string name) : base(nameof(CategoryGroupAttribute))
    {
    }
}



public class SubCategoryAttribute : CategoryAttribute
{
    public SubCategoryAttribute(string name) : base(nameof(SubCategoryAttribute))
    {
    }
}


public class Tag : CategoryAttribute
{
    public Tag(string name) : base(nameof(Tag))
    {
    }
}

