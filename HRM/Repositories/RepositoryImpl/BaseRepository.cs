﻿using HRM.Models;
using Microsoft.EntityFrameworkCore;

namespace HRM.Repositories.RepositoryImpl;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly HrmContext _context;
    protected DbSet<TEntity?> _dbSet;

    public BaseRepository(HrmContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        try
        {
            var context = new HrmContext();
            IQueryable<TEntity?> query = context.Set<TEntity>();
            query = AddIncludes(query);
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task<IEnumerable<TEntity?>> GetAllAsync()
    {
        try
        {
            var context = new HrmContext();
            IQueryable<TEntity?> query = context.Set<TEntity>();
            query = AddIncludes(query);

            return await query.AsNoTracking().ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task<TEntity?> AddAsync(TEntity? entity)
    {
        try
        {
            var context = new HrmContext();
            IQueryable<TEntity?> query = context.Set<TEntity>();
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task UpdateAsync(TEntity? entity)
    {
        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual IQueryable<TEntity?> GetQueryable()
    {
        try
        {
            return _dbSet.AsQueryable();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual IQueryable<TEntity?> AddIncludes(IQueryable<TEntity?> query)
    {
        return query;
    }
}