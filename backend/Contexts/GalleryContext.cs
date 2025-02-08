using Microsoft.EntityFrameworkCore;

namespace backend.Contexts;

public class GalleryContext : DbContext
{
    public GalleryContext(DbContextOptions<GalleryContext> options)
        : base(options)
    {
    } 
}