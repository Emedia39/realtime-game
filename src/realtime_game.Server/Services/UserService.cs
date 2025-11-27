using MagicOnion;
using MagicOnion.Server;//
using Microsoft.EntityFrameworkCore;
using realtime_game.Server.Models.Contexts;
using realtime_game.Shared.Models.Entities;
using realtime_game.Shared.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//インタフェース実装クラス
public class UserService : ServiceBase<IUserService>, IUserService
{
    public async UnaryResult<int> RegistUserAsync(string name)
    {
        using var context = new GameDbContext();
        //バリデーションチェック(名前登録済みかどうか)
        if (context.Users.Count() > 0 &&
              context.Users.Where(user => user.Name == name).Count() > 0)
        {
            throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "");
        }
        //テーブルにレコードを追加
        User user = new User();
        user.Name = name;
        user.Token = "";
        user.Created_at = DateTime.Now;
        user.Updated_at = DateTime.Now;
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.Id;
    }

    // id指定でユーザー情報を取得するAPI
    public async UnaryResult<User> GetUserByIdAsync(int id)
    {
        using var context = new GameDbContext();

        var user = await context.Users.FindAsync(id);

        if (user == null)
        {
            throw new ReturnStatusException(Grpc.Core.StatusCode.NotFound, "User not found.");
        }

        return user;
    }

    // ユーザー一覧を取得するAPI
    public async UnaryResult<User[]> GetUserListAsync()
    {
        using var context = new GameDbContext();

        var users = await context.Users.OrderBy(u => u.Id).ToArrayAsync();
        return users;
    }

    // id指定でユーザー名を更新するAPI
    public async UnaryResult<string> UpdateUserNameAsync(int id, string newName)
    {
        using var context = new GameDbContext();

        var user = await context.Users.FindAsync(id);

        if (user == null)
        {
            throw new ReturnStatusException(Grpc.Core.StatusCode.NotFound, "User not found.");
        }

        // すでに同名ユーザーがいないかチェック
        if (await context.Users.AnyAsync(u => u.Name == newName && u.Id != id))
        {
            throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "Name already exists.");
        }

        user.Name = newName;
        user.Updated_at = DateTime.Now;

        await context.SaveChangesAsync();

        return user.Name;
    }

}



