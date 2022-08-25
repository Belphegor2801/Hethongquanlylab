$(document).ready(function(){
    var dataTinTucJS = new dataJS();
    dataTinTucJS.loadData()
})

class dataJS{
    constructor(){
    }

    loadData(){
        $.each(dataNews, function(index, item){
            var trHTML = $(`<tr>
                                <td> <img src="./img/logo-trắng.jpg" alt="" style="width: 90px;"></td>
                                <td>${item.trươngDuLieu2}</td>
                            </tr>`);
            
            $('.news tbody').append(trHTML);
        })
    }
}

var dataNews = [
    {
        trươngDuLieu2:"Thôhhônng báo 1hônng báo 1hônng g báo 1hhôhông báo 1hônng báo 1hônng báo 1hônng báo 1hônng báo 1hhôhông báo 1hônng báo 1hônng báo 1hônng báo 1hônng nng báo 1hông báo 1 1 ",
    },
    {
        trươngDuLieu2:"Thông báo 2",
    },
    {
        trươngDuLieu2:"Thông báo 3",
    },
    {
        trươngDuLieu2:"Thông báo 4",
    },
]
