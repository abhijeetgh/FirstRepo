var classicViewCarClassId;
var classicViewBrandID;
var classicViewBrandCode;

function RateInfoClassic(item) {
    var self = this;

    var d = convertToServerTime(new Date(parseInt(item.DateInfo.replace("/Date(", "").replace(")/", ""), 10)));
    self.date = new Date(d.getFullYear(), d.getMonth(), d.getDate());
    self.dateInfo = months[d.getMonth()] + '-' + ('0' + (d.getDate())).slice(-2) + '-' + d.getFullYear() + ' <br /> ' + days[d.getDay()].substring(0, 3);
    self.DateFormat = d.getFullYear() + months[d.getMonth()] + d.getDate();
    self.companyDetailsClassic = ko.observableArray($.map(item.CompanyDetails, function (item) { return new CompanyDetailsClassic(item) }));
}

function CompanyDetailsClassic(data) {
    this.companyID = data.CompanyID;
    this.companyCode = data.CompanyCode;
    this.inside = data.Inside;
    this.baseValue = commaSeparateNumber(data.BaseValue);
    this.totalValue = commaSeparateNumber($.isNumeric(data.TotalValue) ? data.TotalValue.toFixed(2) : data.TotalValue);
    this.islowestAmongCompetitors = data.IslowestAmongCompetitors;
}

