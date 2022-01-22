using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_GUI2
{
    class WorldFactory
    {
        // TODO: ワールド一覧の作成
        // ローカルのディレクトリからワールド一覧を表示
        // 登録されているgitアカウントすべてからブランチ名一覧を取得して表示
        // 重複は排除
        // shared world の保存ディレクトリ名は 1.x.x/User.repository.WorldName/ とし衝突を回避(.使える？)

        // shared world のリポジトリ名は User/repository/WorldName としてバージョン情報は記録しない
        // バージョン情報やブランチ一覧等は特別なブランチを作ってjson管理とかがいいか

        // リモートにあってローカルにない
        // User.repository.WorldNameにclone

        // リモートにあってローカルにある
        // User.repository.WorldNameにpull

        // リモートになくてローカルにある
        // 通常起動

        // リモートになくてローカルにない
        // WorldNameを新規作成

        // push先リポジトリを変えたい
        // CustomMapからどうぞ
    }
}
