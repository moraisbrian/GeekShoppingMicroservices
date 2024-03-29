﻿using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Utils;
using System.Net;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _client;
        private const string BasePath = "api/v1/carts";

        public CartService(HttpClient client)
        {
            _client = client;
        }

        public async Task<CartViewModel> AddItemToCart(string token, CartViewModel cart)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsJson($"{BasePath}/add-cart", cart);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartViewModel>();
            else
                throw new Exception("Somthing went wrong when calling API");
        }

        public async Task<bool> ApplyCoupon(string token, CartViewModel cart)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsJson($"{BasePath}/apply-coupon", cart);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else
                throw new Exception("Somthing went wrong when calling API");
        }

        public async Task<object> Checkout(string token, CartHeaderViewModel cartHeader)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsJson($"{BasePath}/checkout", cartHeader);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartHeaderViewModel>();
            else if (response.StatusCode == HttpStatusCode.PreconditionFailed)
                return "Coupon price has changed, please confirm!";
            else
                throw new Exception("Somthing went wrong when calling API");
        }

        public async Task<CartViewModel> FindCartByUserId(string token, string userId)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"{BasePath}/find-cart/{userId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new CartViewModel();

            return await response.ReadContentAs<CartViewModel>();
        }

        public async Task<bool> RemoveCoupon(string token, string userId)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.DeleteAsync($"{BasePath}/remove-coupon/{userId}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else
                throw new Exception("Somthing went wrong when calling API");
        }

        public async Task<bool> RemoveFromCart(string token, long cartId)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.DeleteAsync($"{BasePath}/remove-cart/{cartId}");
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else
                throw new Exception("Somthing went wrong when calling API");
        }

        public async Task<CartViewModel> UpdateCart(string token, CartViewModel cart)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsJson($"{BasePath}/update-cart", cart);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartViewModel>();
            else
                throw new Exception("Somthing went wrong when calling API");
        }
    }
}
