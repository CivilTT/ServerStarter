using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server_GUI2.Develop.Server.World;
using Server_GUI2.Develop.Util;

namespace Server_GUI2.Develop.Server.Storage
{
    /// <summary>
    /// ワールドの保存先(Gitリポジトリ,Gdrive等)
    /// </summary>
    public abstract class Storage
    {
        public abstract WorldWriter GetWorldWriter(Version version);
        public abstract List<LocalWorld> GetWorlds();

        public Storage()
        {
            // WorldFactoryにストレージ内のワールドを登録
            GetWorlds().ForEach( x => WorldFactory.Instance.Worlds.Add(x));
        }
    }

    /// <summary>
    /// gitリモートリポジトリをあらわす　インスタンスはリポジトリの数によって変わる
    /// </summary>
    public class GitStorage: Storage
    {
        public static List<GitStorage> GetStorages()
        {
            var local = new GitLocal(Path.Combine(SetUp.CurrentDirectory,"git"));
            var repos = GitStorageRepository.GetAllGitRepositories(local);
            return repos.Select(x => new GitStorage(x)).ToList();
        }

        public GitStorageRepository Repository;
        public GitStorage(GitStorageRepository repository)
        {
            Repository = repository;
        }

        public override WorldWriter GetWorldWriter(Version version)
        {
            return new GitWorldWriter();
        }

        public override List<LocalWorld> GetWorlds()
        {
            //Repository.GetGitWorlds();
            // TODO: Gitの#stateブランチからworldstate.jsonを取得してワールド一覧のリストを返す
            return new List<LocalWorld>();
        }

        /// <summary>
        /// TODO: ストレージとそれに結び付いたデータを削除
        /// </summary>
        public void Delete()
        {

        }
    }
}
