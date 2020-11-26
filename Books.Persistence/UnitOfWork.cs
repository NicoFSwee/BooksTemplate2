using Books.Core.Contracts;
using Books.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Persistence
{
  public class UnitOfWork : IUnitOfWork
  {
    readonly ApplicationDbContext _dbContext;
    private bool _disposed;

    /// <summary>
    /// ConnectionString kommt aus den appsettings.json
    /// </summary>
    public UnitOfWork() : this(new ApplicationDbContext())
    {
    }
    /// <summary>
    /// ConnectionString kommt aus den appsettings.json
    /// </summary>
    public UnitOfWork(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
      Books = new BookRepository(_dbContext);
      Authors = new AuthorRepository(_dbContext);
    }

    public IBookRepository Books { get; }
    public IAuthorRepository Authors { get; }


    public async Task<int> SaveChangesAsync()
    {
      var entities = _dbContext.ChangeTracker.Entries()
          .Where(entity => entity.State == EntityState.Added
                           || entity.State == EntityState.Modified)
          .Select(e => e.Entity);
      foreach (var entity in entities)
      {
        await ValidateEntity(entity);
      }
      return await _dbContext.SaveChangesAsync();
    }

    public void SyncChangeTrackerCache()
    {
      var entries = _dbContext.ChangeTracker.Entries().ToList();
      entries.ForEach(entry => entry.Reload());
    }

    /// <summary>
    /// Einfache Unique-Überprüfung der ISBN über Validation 
    /// statt über unique index
    /// </summary>
    /// <param name="entity"></param>
    private async Task ValidateEntity(object entity)
    {
      if (entity is Book book)
      {
        var validationContext = new ValidationContext(book);
        // DbContext-Validation als Callback anmelden
        // TODO: book.OnDbContextValidation = CheckIsbnIsUnique;  
        // Validation für Objekt anstossen
        // Etwaige Fehler werden in normale Validierung übernommen
        Validator.ValidateObject(book, validationContext, validateAllProperties: true);
        if (await _dbContext.Books.AnyAsync(b => b.Id != book.Id && b.Isbn == book.Isbn))
        {
          throw new ValidationException(new
              ValidationResult($"Buch mit Isbn {book.Isbn} existiert schon",
                  new List<string> { "Isbn" }), null, new List<string> { "Isbn" });
        }
      }
    }

    public async Task DeleteDatabaseAsync() => await _dbContext.Database.EnsureDeletedAsync();
    public async Task MigrateDatabaseAsync() => await _dbContext.Database.MigrateAsync();
    public async Task CreateDatabaseAsync() => await _dbContext.Database.EnsureCreatedAsync();

    public async ValueTask DisposeAsync()
    {
      await DisposeAsync(true);
      GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
        {
          await _dbContext.DisposeAsync();
        }
      }
      _disposed = true;
    }

  }
}
