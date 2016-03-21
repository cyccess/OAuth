using AutoMapper;
using EntityFramework.Extensions;
using OAuth.Core.Interfaces;
using OAuth.Domain.Model;
using OAuth.Service.Common;
using OAuth.Service.Interfaces;
using OAuth.Service.ModelDto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using OAuth.Domain.IRepository;
using Webdiyer.WebControls.Mvc;

namespace OAuth.Service
{
    public class UserService : IUserService
    {
        private readonly IRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IUserRepository _userRepository;

        public UserService(IRepository repo, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }


        public User Get(int id)
        {
            return _userRepository.Get(id);
        }


        public UserDto GetUesrById(int id)
        {
            var entity = _repo.GetById<User>(id);
            var userDto = Mapper.Map<User, UserDto>(entity);
            return userDto;
        }

        public User GetUesr(int id)
        {
            var entity = _repo
                .GetAll<User>()
                .Include(u => u.UserRoles.Select(r => r.Role))
                .Include(u => u.UserProjects.Select(up => up.Project)).Single(u => u.Id == id);
            return entity;
        }

        public User Add(User entity)
        {
            if (_repo.GetAll<User>().Any(u => u.UserName == entity.UserName))
            {
                throw new ArgumentException("username has already existed");
            }

            entity.Password = EncryptHelper.Encrypt(entity.Password);

            if (entity.Password == "")
            {
                throw new ArgumentException("服务器出错，请稍后再试！");
            }
            entity.Status = 1;
            entity.AddDate = DateTime.Now;

            //使用UnitOfWork方式
            _unitOfWork.RegisterNew(entity);
            _unitOfWork.Commit();

            return entity;
        }

        public IPagedList<User> GetUsers(int pageIndex)
        {
            var pageList = _repo.GetAll<User>().Include(u => u.UserRoles.Select(r => r.Role))
                .Where(u => u.Status == 1)
                .OrderBy(u => u.Id)
                .ToPagedList(pageIndex, 10);
            return pageList;
        }

        public void Update(UserDto user)
        {
            //使用 EntityFramework.Extensions 新用法
            //var users = _repo.GetAll<User>();
            //users = users.Where(u => u.Id == user.Id);
            //users.Update<User>(u => new User
            //{
            //    UserName = user.UserName,
            //    FullName = user.FullName,
            //    PhoneNumber = user.PhoneNumber
            //});


            //使用UnitOfWork方式
            var entity = _repo.GetAll<User>().Single(u => u.Id == user.Id);
            entity.UserName = user.UserName;
            entity.FullName = user.FullName;
            entity.PhoneNumber = user.PhoneNumber;

            _unitOfWork.RegisterDirty(entity);
            _unitOfWork.Commit();

            //使用Repository的SaveChanges
            //var entity = repo.GetById<User>(user.Id);

            //entity.UserName = user.UserName;
            //entity.FullName = user.FullName;
            //entity.PhoneNumber = user.PhoneNumber;

            //repo.Update(entity);
            //repo.SaveChanges();
        }

        public void Delate(int id)
        {
            if (id == 0)
            {
                // ReSharper disable once ObjectCreationAsStatement
                new ArgumentException("传入的Id错误");
            }

            //使用 EntityFramework.Extensions 新用法
            //var users = _repo.GetAll<User>();
            //users = users.Where(u => u.Id == id);
            //users.Delete<User>();

            var user = _repo.GetAll<User>().Single(u => u.Id == id);
            _unitOfWork.RegisterDeleted(user);
            _unitOfWork.Commit();


            //var entity = repo.GetById<User>(id);
            //if (entity != null)
            //{
            //    repo.Delete(entity);
            //    repo.SaveChanges();
            //}
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserDto Login(string username, string password)
        {
            //if (string.IsNullOrEmpty(username))
            //{
            //    throw new ArgumentException("用户名输入错误");
            //}

            password = EncryptHelper.Encrypt(password);
            var entity = _repo.GetAll<User>().SingleOrDefault(u => u.UserName == username && u.Password == password);

            if (entity != null)
            {
                var userDto = Mapper.Map<User, UserDto>(entity);
                userDto.IsDisabled = userDto.Status != 1;
                return userDto;
            }

            return new UserDto();
        }

        /// <summary>
        /// 获取用户拥有的项目集合
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        public IEnumerable<Project> GetUserProjectList(int userId)
        {
            var projectList = _repo.GetAll<User>()
                .Where(u => u.Id == userId)
                .Select(u => u.UserProjects.Select(p => p.Project)).FirstOrDefault();

            return projectList;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        public void ResetPassword(int uid, string password)
        {
            var entity = _repo.GetById<User>(uid);
            password = EncryptHelper.Encrypt(password);
            entity.Password = password;

            _unitOfWork.Commit();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="newPwd"></param>
        /// <param name="oldPwd"></param>
        public ResultModel ChangePassword(int uid, string newPwd, string oldPwd)
        {
            ResultModel resultModel;
            oldPwd = EncryptHelper.Encrypt(oldPwd);
            var entity = _repo.GetAll<User>().SingleOrDefault(u => u.Id == uid && u.Password == oldPwd);

            if (entity == null)
            {
                resultModel = new ResultModel(-100, "旧密码验证错误");
                return resultModel;
            }
            newPwd = EncryptHelper.Encrypt(newPwd);
            entity.Password = newPwd;
            _unitOfWork.Commit();
            resultModel = new ResultModel(200, "密码修改成功");
            return resultModel;
        }

        /// <summary>
        /// 设置用户项目
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="projectId"></param>
        public void SetUserProject(int userId, int projectId)
        {
            var entity = _repo.GetAll<UserProject>().FirstOrDefault(p => p.UserId == userId && p.ProjectId == projectId);

            if (entity != null)
            {
                _unitOfWork.RegisterDeleted(entity);
            }
            else
            {
                entity = new UserProject
                {
                    UserId = userId,
                    ProjectId = projectId
                };
                _unitOfWork.RegisterNew(entity);
            }
            _unitOfWork.Commit();
        }
    }
}
