$(function(){
    $('.uNewsGoogleMapDiv').each(function(){
        var $mapDiv=$(this);        
        var map=new Map($mapDiv.attr('lat'), $mapDiv.attr('lng'), $mapDiv.attr('zoom'), google.maps.MapTypeId.ROADMAP, $mapDiv);
        
        $mapDiv.parent().find('.uNewsGoogleMapSearchBox').keyup(function(){
            buildXML(map.mapDiv, map.map);
        });
        
        $mapDiv.parent().find('.uNewsGoogleMapSearchButton').click(function(){
            var $address=$(this).parent().find('.uNewsGoogleMapSearchBox');
            geocode($address.val(), $mapDiv);
        });
    });
});

function geocode(address, $mapDiv){
    if(address!=''){
        var geocoder=new google.maps.Geocoder();
        geocoder.geocode({'address': address}, function(results, status){
          if(status==google.maps.GeocoderStatus.OK){
            
            var lat=results[0].geometry.location.lat();
            var lng=results[0].geometry.location.lng();
            
            for(i in results[0]['address_components']){
              if(results[0]['address_components'][i]['types'][0]=='locality'){
                var city=results[0]['address_components'][i]['long_name'];
              }
            }
            for(i in results[0]['address_components']){
              if(results[0]['address_components'][i]['types'][0]=='administrative_area_level_1'){
                var state=results[0]['address_components'][i]['short_name'];
                var stateFullname=results[0]['address_components'][i]['long_name'];
              }
            }
            
            //getSalespersons(state, stateFullname);
            var map=new Map(lat, lng, $mapDiv.attr('zoom'), google.maps.MapTypeId.ROADMAP, $mapDiv);
          }
        });
    } else {
        alert('Please enter your address!');
        return false;
    }
}

function buildXML(mapDiv, map){
    try{
        var $mapDiv=$(mapDiv);
        var $saveBox=$mapDiv.siblings('.uNewsGoogleMapSaveBox');
        
        var xml='<googleMap>';
        
        xml+='<coords lat="'+map.getCenter().lat()+'" lng="'+map.getCenter().lng()+'"/>';
        xml+='<zoom>'+map.zoom+'</zoom>';
        xml+='<address>'+htmlEntities($mapDiv.parent().find('.uNewsGoogleMapSearchBox').val()).replace(/\n/g, '<br/>')+'</address>';
        
        xml+='</googleMap>';
        
        $saveBox.val(xml);
    } catch (e){
        //console.log(e);
    }
}

function Map(givenLat, givenLng, givenZoom, givenType, $mapDiv){

    //set up the map options
    var center = new google.maps.LatLng(givenLat, givenLng);
    var mapOptions = {
        zoom: parseInt(givenZoom),
        center: center,
        mapTypeControl: false,
        mapTypeControlOptions: {style: google.maps.MapTypeControlStyle.DROPDOWN_MENU},
        navigationControl: true,
        navigationControlOptions: {style: google.maps.NavigationControlStyle.SMALL},
        mapTypeId: givenType,
        scrollwheel: false
    };
    this.mapDiv=$mapDiv[0];
     
    this.map=new google.maps.Map(this.mapDiv, mapOptions);//create the map
    
    //keep references
    var mapRef=this;
    var mapDivRef=this.mapDiv;
    
    this.markers = new Array();

    this.getMarkers = function(){
        return this.markers
    };

    this.clearMarkers = function(){
        for(var i=0; i<this.markers.length; i++){
            this.markers[i].setMap(null);
        }
        this.markers = new Array();
    };
        
    this.addMarker=function(location, draggable){
    
        var marker = new google.maps.Marker({
            position: location,
            map: this.map,
            draggable: draggable
        });    
        this.markers[this.markers.length] = marker;

        //if dragable set the dragend event
        if(draggable){
            google.maps.event.addListener(marker, 'dragend', function(){
                //map.setCenter(marker.getPosition());
                window.setTimeout(function(){
                  mapRef.map.panTo(marker.getPosition());
                  buildXML(mapDivRef, mapRef.map);
                }, 1000);
            });
        }
    }
    
    google.maps.event.addListener(this.map, 'click', function(event){
        mapRef.clearMarkers();
        mapRef.addMarker(event.latLng, true);
        
        window.setTimeout(function(){
            mapRef.map.panTo(event.latLng);
            buildXML(mapDivRef, mapRef.map);
        }, 1000);
    });
    
    google.maps.event.addListener(this.map, 'zoom_changed', function(event){
        buildXML(mapDivRef, mapRef.map);
    }); 
    
    this.addMarker(new google.maps.LatLng(givenLat, givenLng), true); 
    buildXML(mapDivRef, mapRef.map);

    return this;
}

function htmlEntities(string){
    return String(string).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}