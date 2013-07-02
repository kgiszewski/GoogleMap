$(function(){
    
    $('input').keyup(function(){
        buildJSON();
    });
    
    //ini
    buildJSON();
});

function buildJSON(){
    
    var $saveBox=$('.saveBox');
    
    var $address=$('.address');
    var $lat=$('.lat');
    var $lng=$('.lng');
    var $zoom=$('.zoom');
    var $height=$('.height');
    var $width=$('.width');
    
    var json=[];
    
    json.push('"address":"'+encodeURI($address.val())+'"');
    json.push('"lat":'+(parseFloat($lat.val())||40.718119));
    json.push('"lng":'+(parseFloat($lng.val())||-74.004135));
    json.push('"zoom":'+(parseInt($zoom.val())||15));
    json.push('"width":'+(parseInt($width.val())||300));
    json.push('"height":'+(parseInt($height.val())||300));    
    
    $saveBox.val("{"+json.join(',')+"}");
}