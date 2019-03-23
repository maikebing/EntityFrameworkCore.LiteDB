using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteDBConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new MyContext())
            {
                context.Database.EnsureCreated();
                if (context.Users.Any())
                {
                    context.RemoveRange(context.Users.ToArray());
                }
                if (context.Blogs.Any())
                {
                    context.RemoveRange(context.Blogs.ToArray());
                }

                var user = new User { Name = "Yuuko" };
                context.Add(user);
                var blog1 = new Blog
                {
                    Title = "Title #1",
                    User = user,
                    Content = "ASP.NET Core LiteDB EFCore "
                };
                context.Add(blog1);
                var blog2 = new Blog
                {
                    Title = "Title #2",
                    User = user,
                    Content = DateTime.Now.ToString()
                };
                context.Add(blog2);
                context.SaveChanges();
                var ret = context.Blogs
                    .Where(x => x.Content.Contains("LiteDB"))
                    .ToList();
                foreach (var x in ret)
                {
                    Console.WriteLine($"{ x.Id } { x.Title }");
                    Console.WriteLine();
                }
            }
            Console.Read();
        }
    }
}
