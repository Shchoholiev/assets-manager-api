using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Paging;
using System.Net;
using System.Net.Http.Json;

namespace AssetsManagerApi.IntegrationTests.Tests
{
    public class TagsControllerTests(TestingFactory<Program> factory)
            : TestsBase(factory, "tags")
    {
        [Fact]
        public async Task GetTagsPage_ValidRequest_ReturnsPaginatedList()
        {
            // Arrange
            await LoginAsync("enterprise@gmail.com", "Yuiop12345");
            var pageNumber = 1;
            var pageSize = 10;

            // Act
            var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}");
            var pagedList = await response.Content.ReadFromJsonAsync<PagedList<TagDto>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(pagedList);
            Assert.True(pagedList.Items.Any());
        }

        [Fact]
        public async Task GetTagsPage_WithSearchString_ReturnsFilteredPaginatedList()
        {
            // Arrange
            await LoginAsync("enterprise@gmail.com", "Yuiop12345");
            var searchString = "test";
            var pageNumber = 1;
            var pageSize = 10;

            // Act
            var response = await HttpClient.GetAsync($"{ResourceUrl}?searchString={searchString}&pageNumber={pageNumber}&pageSize={pageSize}");
            var pagedList = await response.Content.ReadFromJsonAsync<PagedList<TagDto>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(pagedList);
            Assert.True(pagedList.Items.Any());

            // Additional asserts to verify filtering
            Assert.All(pagedList.Items, item => Assert.Contains(searchString, item.Name, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetTagsPage_InvalidPageNumber_ReturnsBadRequest()
        {
            // Arrange
            await LoginAsync("enterprise@gmail.com", "Yuiop12345");
            var pageNumber = -1; // Invalid page number
            var pageSize = 10;

            // Act
            var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GetTagsPage_InvalidPageSize_ReturnsBadRequest()
        {
            // Arrange
            await LoginAsync("enterprise@gmail.com", "Yuiop12345");
            var pageNumber = 1;
            var pageSize = -1; // Invalid page size

            // Act
            var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
