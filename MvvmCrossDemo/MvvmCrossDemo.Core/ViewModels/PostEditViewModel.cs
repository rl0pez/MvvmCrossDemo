﻿using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvvmCrossDemo.Core.Models;
using MvvmCrossDemo.Core.Models.Dto;
using MvvmCrossDemo.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Commands;

namespace MvvmCrossDemo.Core.ViewModels
{
    public class PostEditViewModel : MvxViewModel<PostViewModel, Post>
    {
        private readonly IPostService _postService;
        private readonly IMvxNavigationService _navigationService;

        private int _postId;


        public PostEditViewModel(IPostService postService, IMvxNavigationService navigationService)
        {
            _postService = postService;
            _navigationService = navigationService;
        }

        public override void Prepare(PostViewModel post)
        {
            // This is the first method to be called after construction
            _postId = post.Id;
        }

        public override async Task Initialize()
        {
            // Async initialization, Aha!

            await base.Initialize();
            await GetPost(_postId);
        }


        #region Post;
        private PostViewModel _post;
        public PostViewModel Post
        {
            get => _post;
            set => SetProperty(ref _post, value);
        }
        #endregion


        private async Task GetPost(int postId)
        {
            var response = await _postService.GetPost(postId);
            if (response.IsSuccess)
            {
                Post = AutoMapper.Mapper.Map<PostViewModel>(response.Result);
            }
        }




        #region CancelAsyncCommand;
        private IMvxAsyncCommand _cancelAsyncCommand;
        public IMvxAsyncCommand CancelAsyncCommand
        {
            get
            {
                _cancelAsyncCommand = _cancelAsyncCommand ?? new MvxAsyncCommand(CancelAsync);
                return _cancelAsyncCommand;
            }
        }
        private async Task CancelAsync()
        {
            // Implement your logic here.
            await _navigationService.Close(this);
        }
        #endregion


        //#region EditPostAsyncCommand;
        //private IMvxAsyncCommand _editPostAsyncCommand;
        //public IMvxAsyncCommand EditPostAsyncCommand
        //{
        //    get
        //    {
        //        _editPostAsyncCommand = _editPostAsyncCommand ?? new MvxAsyncCommand(EditPostAsync);
        //        return _editPostAsyncCommand;
        //    }
        //}
        //private async Task EditPostAsync()
        //{
        //    // Implement your logic here.
        //    var response = await _postService.UpdatePost(Post.Id, AutoMapper.Mapper.Map<Post>(Post));
        //    if (response.IsSuccess)
        //    {
        //        await _navigationService.Close(this, response.Result);
        //    }
        //}
        //#endregion




        #region EditPostAsyncCommand;

        #region EditPostTaskNotifier;
        private MvxNotifyTask _editPostTaskNotifier;
        /// <summary>
        /// Use the IsNotCompleted/IsCompleted properties of the MvxNotifyTask to show an indicator. Using the MvxNotifyTask is a recommended way to use an async command.
        /// </summary>
        /// <value>
        /// The edit post task notifier.
        /// </value>
        public MvxNotifyTask EditPostTaskNotifier
        {
            get => _editPostTaskNotifier;
            set => SetProperty(ref _editPostTaskNotifier, value);
        }
        #endregion

        private IMvxCommand _editPostAsyncCommand;
        public IMvxCommand EditPostAsyncCommand
        {
            get
            {
                _editPostAsyncCommand = _editPostAsyncCommand ?? new MvxCommand(() =>
                {
                    EditPostTaskNotifier = MvxNotifyTask.Create(async () =>
                        {
                            await EditPostAsync();
                        },
                        OnEditPostException);
                });
                return _editPostAsyncCommand;
            }
        }
        private async Task EditPostAsync()
        {
            // Implement your logic here.
            var response = await _postService
                .UpdatePost(Post.Id, AutoMapper.Mapper.Map<Post>(Post));
            if (response.IsSuccess)
            {
                await _navigationService.Close(this, response.Result);
            }
        }

        private void OnEditPostException(Exception ex)
        {
            // Catch and log the exception here.
        }
        #endregion
    }
}
