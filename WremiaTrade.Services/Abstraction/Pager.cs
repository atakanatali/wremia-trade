namespace Papara.Services.Abstraction
{
    using System;
    using System.Reflection;

    using Papara.Utilities;

    //TODO: Delete all things that are related with old pager
    public class Pager<TRequestModelType> where TRequestModelType : QueryRequestModel, new()
    {
        public Pager(string action, string controller)
        {
            PagerAction = action;
            PagerController = controller;
            Page = 1;
            PageItemCount = 20;
            RequestModel = new TRequestModelType();
        }

        private TRequestModelType requestModel;

        /// <summary>
        /// Action
        /// </summary>
        public string PagerAction { get; set; }

        /// <summary>
        /// Controller
        /// </summary>
        public string PagerController { get; set; }

        /// <summary>
        /// Request model that you will use in request to controller to get records. 
        /// This is actually for searching.
        /// </summary>
        public TRequestModelType RequestModel
        {
            get
            {
                requestModel.Skip = PageSkip;
                requestModel.Take = PageItemCount;
                return requestModel;
            }
            set
            {
                requestModel = value;
            }
        }

        /// <summary>
        /// Current page
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// How many record do you want to show on page
        /// </summary>
        public int PageItemCount { get; set; }

        /// <summary>
        /// How many record there is in database according to request model
        /// </summary>
        public int TotalItemCount { get; set; }

        /// <summary>
        /// Total page count according to records and page item count
        /// </summary>
        public int TotalPageCount => PagingCalculator.GetPageCount(this.TotalItemCount, this.PageItemCount);

        public int PageSkip => (Page * PageItemCount) - PageItemCount;

        /// <summary>
        /// Generates paging as html
        /// </summary>
        public Microsoft.AspNetCore.Html.HtmlString GeneratePaging()
        {
            var pagingHtml = "";

            var showPages = 6;

            var url = $"/{PagerController}/{PagerAction}?";

            foreach (PropertyInfo property in RequestModel.GetType().GetProperties())
            {
                object propValue = property.GetValue(RequestModel);

                if (property.Name == "Skip" || property.Name == "Take")
                {
                    continue;
                }

                if (propValue == null)
                {
                    continue;
                }

                if (propValue.GetType().IsArray)
                {
                    foreach (object prop in propValue as Array)
                    {
                        url += $"RequestModel.{property.Name}={prop}&";
                    }

                    continue;
                }

                url += $"RequestModel.{property.Name}={propValue}&";
            }

            if (Page > 1 && TotalPageCount > showPages)
            {
                pagingHtml = $@"<a href='{url}Page=1'>1</a><a href='{url}Page={Page - 1}'><</a>";
            }

            var count = 1;

            int loop = Page;

            int difPages = TotalPageCount - Page;

            if (difPages < 0)
            {
                loop = Page;
            }
            if (TotalPageCount < showPages)
            {
                loop = 1;
            }
            else if (difPages == 0 && TotalPageCount >= showPages)
            {
                loop = TotalPageCount - (showPages - 1);
            }
            else if (difPages < showPages && Page >= showPages)
            {
                loop = TotalPageCount - (showPages - 1);
            }
            else if (difPages > showPages)
            {
                loop = Page;
            }
            else if (TotalPageCount <= showPages)
            {
                loop = 1;
            }
            else
            {
                loop = Page;
            }

            for (int i = loop; i <= TotalPageCount; i++)
            {

                var activeClass = "";

                if (i == Page)
                {
                    activeClass = "active";
                }

                pagingHtml = pagingHtml + $"<a class='{activeClass}' href='{url}Page={i}'>{i}</a>";

                count++;

                if (count >= showPages)
                {
                    if (Page + showPages <= TotalPageCount)
                    {
                        pagingHtml = pagingHtml + $"<a href='{url}Page={Page + 1}'>></a><a href='{url}Page={TotalPageCount}'>" + TotalPageCount + "</a>";
                        break;
                    }
                }
            }

            return new Microsoft.AspNetCore.Html.HtmlString($"<div class='paparaPaging'>{pagingHtml}<label>Toplam Kayıt: {TotalItemCount}</label></div>");
        }
    }
}
