using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.WCP.Common.Primitives;
using ASiNet.WCP.History.Actions;

namespace ASiNet.WCP.History;
public class WCPHistory
{

    public PostHistoryResponse PostHistory(PostHistoryRequest request)
    {
        try
        {
            using var context = new HistoryContext();
            var result = context.AddRecord(new TextRecord(request.Item!.Text!, request.Item.SendedTime));
            return new() { Id = result.Id };
        }
        catch (Exception)
        {
            return new() { Id = 0 };
        }
    }

    public GetHistoryResponse GetHistory(GetHistoryRequest request)
    {
        try
        {
            using var context = new HistoryContext();

            var result = context.Records.OrderByDescending(x => x.Id).Where(x => x.Id < request.StartId).Take(request.MaxCount).ToArray();
            var r = new GetHistoryResponse()
            {
                Items = [.. result.Select(x => new HistoryItem()
                {
                    Id = x.Id,
                    ItemType = Common.Enums.HistoryItemType.Text, SendedTime = x.CreatedTime,
                    Text = ((TextRecord)x).Text
                }).GroupBy(x => DateOnly.FromDateTime(x.SendedTime)).FirstOrDefault()]
            };
            return r;
        }
        catch (Exception)
        {
            return new();
        }
    }

}
