using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;

namespace WebApplication2.Models;

public class Repository<T> : IRepository<T> where T : class
{
    private  ApplicationDbContext _context { get; set; }
    private  DbSet<T> _DbSet { get; set; }

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _DbSet = _context.Set<T>();
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _DbSet.ToListAsync();
    }

    public Task<T> GetByIdAsync(int id, QueryOptions<T> options)
    {
        IQueryable<T> query = _DbSet;
        if (options.HasWhere())
        {
            query = query.Where(options.Where);
        }

        if (options.HasOrderBy())
        {
            query = query.OrderBy(options.OrderBy);
        }

        foreach (string include in options.GetIncludes())
        {
            query = query.Include(include);
        }
        var key = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.FirstOrDefault();
        string primaryKeyName = key?.Name;
        return query.FirstOrDefaultAsync(e => EF.Property<int>(e, primaryKeyName) == id);
    }

    public async Task AddAsync(T entity)
    {
        await _DbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        T entity = await _DbSet.FindAsync(id);
        _DbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<T>> GetAllByIdAsync<TKey>(TKey id, string propertyName, QueryOptions<T> options)
    {
        IQueryable<T> query = _DbSet;

        if (options.HasWhere())
        {
            query = query.Where(options.Where);
        }


        if (options.HasOrderBy())
        {
            query = query.OrderBy(options.OrderBy);
        }

        foreach (string include in options.GetIncludes())
        {
            query = query.Include(include);
        }
        // Filter by the specified property name and id
        query = query.Where(e => EF.Property<TKey>(e, propertyName).Equals(id));

        return await query.ToListAsync();

    }
}