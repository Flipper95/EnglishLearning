	window.onload =
    $(document).ready(
        function () {
	    if($(window).width() < 770){
	    	$(".listByGroup").css("columns", "1");
		$(".treeNode").css("width", "100%");
	    }
        });
	
	window.onresize = () => {
		if($(window).width() < 770){
			$(".listByGroup").css("columns", "1");
			$(".treeNode").css("width", "100%");
		}
		else{
			$(".listByGroup").css("columns", "2");
			$(".treeNode").css("width", "50%");
		}
	};