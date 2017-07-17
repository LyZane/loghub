function ButtonReset(btn) {
    btn.html(btn.attr("data-temp")).removeAttr("disabled").attr("working", "false");
}
function ButtonEvent_Get(btn, text, url,data, callback_success, callback_error) {
    if (btn.attr("working") == "true") {
        btn.html("请耐心等待...");
        return;
    }
    btn.attr("data-temp", btn.html()).attr("disabled", "disabled").html(text).attr("working", "true");
    $.ajax({ url: url,data:data, type: "get", success: callback_success, error: callback_error });
}
function ButtonEvent_Post(btn, text, url, data, callback_success, callback_error) {
    text = text ? text : "<img src='/resources/img/ajax-loaders/ajax-loader-1.gif'>";
    if (btn.attr("working") == "true") {
        btn.html("请耐心等待...");
        return;
    }
    btn.attr("data-temp", btn.html()).attr("disabled", "disabled").html(text).attr("working", "true");
    $.ajax({ url: url, data: data, type: "post", success: callback_success, error: callback_error });
}
function insertText(obj, str) {
    if (document.selection) {
        var sel = document.selection.createRange();
        sel.text = str;
    } else if (typeof obj.selectionStart === 'number' && typeof obj.selectionEnd === 'number') {
        var startPos = obj.selectionStart,
            endPos = obj.selectionEnd,
            cursorPos = startPos,
            tmpStr = obj.value;
        obj.value = tmpStr.substring(0, startPos) + str + tmpStr.substring(endPos, tmpStr.length);
        cursorPos += str.length;
        obj.selectionStart = obj.selectionEnd = cursorPos;
    } else {
        obj.value += str;
    }
}

window.Now = function () {
    return new Date();
}
Date.prototype.AddDays = function (count) {
    count = parseInt(count);
    this.setDate(this.getDate() + count);
    return this;
}
// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

Date.prototype.ToString = function () {
    var year = this.getFullYear();
    var month = this.getMonth() + 1;//获取当前月份的日期
    var day = this.getDate();
    if (month < 10) {
        month = "0" + month;
    }
    if (day < 10) {
        day = "0" + day;
    }
    return year + "-" + month + "-" + day;
}

// 从左至右截取字符串，直到遇到subStr（不包含subStr）
String.prototype.SubString_Left = function(subStr)
{
    if (this.indexOf(subStr) > 0) {
        return this.substring(0, this.indexOf(subStr));
    }
    else {
        return this;
    }
}
//从第一个subStr开始截取字符串，直到结尾（不包含subStr）
String.prototype.SubString_Right = function(subStr)
{
    if (this.indexOf(subStr) > 0) {
        return this.substring(this.indexOf(subStr) + 1, this.length);
    }
    else {
        return this;
    }
}
//从最后一个subStr开始截取字符串，直到结尾（不包含subStr）
String.prototype.SubString_Right_Last = function(subStr)
{
    if (this.indexOf(subStr) > 0) {
        return this.substring(this.lastIndexOf(subStr) + 1, this.length);
    }
    else {
        return this;
    }
}
//从左至右截取字符串，直到遇到最后一个subStr（不包含subStr）
String.prototype.SubString_Left_Last = function(subStr)
{
    if (this.indexOf(subStr) > 0) {
        return this.substring(0, this.lastIndexOf(subStr));
    }
    else {
        return this;
    }
}
//从左往右获取字符串指定长度
String.prototype.Left = function(length)
{
    if (this == null) {
        return this;
    }
    else {
        if (length < 0) {
            length = this.length + length;
        }
        length = length < 0 ? 0 : length;
        if (this.length <= length) {
            return this;
        }
        else {
            return this.substring(0, length);
        }
    }
}
//从右往左获取字符串指定长度
String.prototype.Right = function(length) {
    if (this == null) return this;
    if (length < 0) {
        length = this.length + length;
    }
    length = length < 0 ? 0 : length;
    if (this.length <= length) {
        return this;
    }
    else {
        return this.substring(this.length - length, length);
    }
}

function parseURL(url) {
    var a = document.createElement('a');
    a.href = url;
    return {
        source: url,
        protocol: a.protocol.replace(':', ''),
        host: a.hostname,
        port: a.port,
        query: a.search,
        params: (function () {
            var ret = {},
                seg = a.search.replace(/^\?/, '').split('&'),
                len = seg.length, i = 0, s;
            for (; i < len; i++) {
                if (!seg[i]) { continue; }
                s = seg[i].split('=');
                ret[s[0]] = decodeURIComponent(s[1]);
            }
            return ret;
        })(),
        file: (a.pathname.match(/\/([^\/?#]+)$/i) || [, ''])[1],
        hash: a.hash.replace('#', ''),
        path: a.pathname.replace(/^([^\/])/, '/$1'),
        relative: (a.href.match(/tps?:\/\/[^\/]+(.+)/) || [, ''])[1],
        segments: a.pathname.replace(/^\//, '').split('/')
    };
}

function Template2HTML(template, data) {
    return template.replace(/\{[^\{\}]+\}/g, function (key) {
        var value = data[key.match(/[^\{\}]+/)];
        value = value == null ? "" : value;
        value = /^\d{4}-\d{1,2}-\d{1,2}/.test(value) ? new Date(value.replace("T"," ")).Format("yyyy年MM月dd日 hh:mm:ss") : value;
        return value;
    });
}

function guid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function SubmitForm(form, btn,text, callback) {
    var data = form.serialize();
    if (form.attr("method")=="post") {
        ButtonEvent_Post(btn, text, form.attr("action"), data, callback);
    } else {
        ButtonEvent_Get(btn, text, form.attr("action"), data, callback);
    }
    
}