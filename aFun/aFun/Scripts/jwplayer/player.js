function reLoad(url){
	jwplayer().load([{file: url}]);
	jwplayer().play();
}	
if(responseText == 0){
	$('.warning_login').show();
}else{
	urlArry = responseText.split(',');
	start = true;
	num_of_urlArry = urlArry.length;
	index_of_urlArry = 0;
}
jwplayer('myElement').setup({
	file: urlArry[0],
	width: '100%',
	aspectratio: "16:9",
	autostart: true,
	events: {
		onError : function (event) {
			/*--case1: Box error up khi là lần mở video đầu tiên.
					- ta sẽ load một luồng khác thay thế luồng cũ.
			*/
			if(start == true)
			{
				index_of_urlArry = index_of_urlArry + 1;
				if(index_of_urlArry <= num_of_urlArry){
					reLoad(urlArry[index_of_urlArry]);
				}
				
			}
			/*--case2: Box error up khi video đã chạy được một thời gian.
					 - ta sẽ load lại luồng hiện tại đang chạy.
			*/
			else
			{
				reLoad(urlArry[index_of_urlArry]);
				
			}
		},
		onBuffer: function (event){
			//-- khi bắt đầu trạng thái xoay ta sẽ thiết lập ngừng play
			playing = false;
			
			/*-- khi vào trạng thái này ta sẽ đếm ngược trong 10s
					nếu sau 10s mà sét trạng thái play vẫn là fase thì có nghĩa là vẫn bị
					quay video lúc này ta sẽ load lại stream.
			*/
			setTimeout(function(){
				if(playing == false){
					//--case1: ở lần mở đầu tiên đã bị quay đơ - load lại stream mới
					if(start == true){
						index_of_urlArry = index_of_urlArry + 1;
				
						if(index_of_urlArry <= num_of_urlArry){
							reLoad(urlArry[index_of_urlArry]);
						}
					//--case2: đã play được một lúc rồi bị quay đơ - load lại stream cũ.
					}else{
						reLoad(urlArry[index_of_urlArry]);
					}
				}
				//alert(playing);
			},20000);
		},
		onPlay: function (event){
			//-- khi đã một lần vào trạng thái này thì sẽ thiết lập khồng còn là lần chạy đầu tiên, và đang ở trạng thái chạy
			start = false;
			playing = true;
		}
	}
});