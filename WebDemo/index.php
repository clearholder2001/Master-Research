<html>
<head>
<meta charset="utf-8" />
<title>Consistent Roof Geometry Encoding for 3D Building Model Retrieval Using Airborne LiDAR Point Clouds</title>
<script language="javascript" type="text/javascript" src="js/jquery-1.7.1.js"></script>
<style TYPE="text/css">
A:link { color: blue; text-decoration: none }
A:visited { color: blue; text-decoration: none }
A:active { text-decoration: none }
A:hover { color: red; text-decoration: underline}
</style>
<link href='css\tab.css' rel='stylesheet' type='text/css'>
<script language="javascript">
function DoQuery()
{
	var Qres=document.getElementById('Qresult');
	var Qlog=document.getElementById('Qlog');
	Qres.innerHTML="";
	Qlog.innerHTML="Start...";

	var fn=document.getElementById('file').value;
	var type=fn.split(".");
	if(fn=="")
	{
		Qlog.innerHTML="No file is selected.";
		return;
	}
	if(type[type.length-1]=="xyz" || type[type.length-1] == "obj")
	{
		document.form2.submit();
		$("#frame_upload").load(function()
		{
			var frame=document.getElementById('frame_upload').contentWindow.document;
			var HTML=frame.body.innerHTML;
			var return_Value=frame.getElementById('return_Value').value;
			var return_message;
			if(return_Value == 1)
			{
				DoSQL();
				return_message="Query Success."
			}
			else
				return_message="Error.<br>Return Code: "+return_Value;
			Qlog.innerHTML=HTML+return_message;
		});	
	}
	else
	{
		Qres.innerHTML="";
		Qlog.innerHTML="Only *.xyz and *.obj files is accepted.";
		//Qlog.innerHTML="Only *.xyz file is accepted.";
	}
}
var DoSQL=function()
{
	var Qres=document.getElementById('Qresult');
	var Qlog=document.getElementById('Qlog');
	var URLs='search.php';
	$.ajax(
	{
		url: URLs,
		data: null,
		type:'POST',
		dataType:'text',
		success: function(msg)
		{
			Qres.innerHTML="<br><br>"+msg;
		},
		error:function(xhr, ajaxOptions, thrownError)
		{ 
			alert(xhr.status); 
			alert(thrownError); 
		}
	});
}
var DoRequery=function(Val)
{
	
	var Qres=document.getElementById('Qresult');
	var Qlog=document.getElementById('Qlog');
	
	document.getElementById('id').value=Val;
	Qres.innerHTML="";
	Qlog.innerHTML="Start...";
	$.ajax(
	{
		url: 'requery.php',
		data: $('#form1').serialize(),
		type:'POST',
		dataType:'text',
		success: function(msg)
		{
			Qlog.innerHTML=msg;
			DoSQL();
		},
		error:function(xhr, ajaxOptions, thrownError)
		{ 
			alert(xhr.status); 
			alert(thrownError); 
		}
	});
}

$(function(){
	/*換成英文Upload*/
	$("#upload_area").click(function () {
			$("#file").click();
	});
	
  $('#file').change(function(){
		var filename = $('input[type=file]').val().split('\\').pop();
		if(filename == "")
			filename="Click to upload model (<b>xyz</b> and <b>obj</b> format)";	
			//filename="Click to upload model (<b>xyz</b> format)";		
		else
			filename="File: <b>" + filename+"</b>";
		$('#upload_area').html(filename);
   });
	 
	 
	 /*Tab 頁籤區*/
	 // 預設顯示第一個 Tab
	var _showTab = 0;
	var $defaultLi = $('ul.tabs li').eq(_showTab).addClass('active');
	$($defaultLi.find('a').attr('href')).siblings().hide();
 
	// 當 li 頁籤被點擊時...
	// 若要改成滑鼠移到 li 頁籤就切換時, 把 click 改成 mouseover
	$('ul.tabs li').click(function() {
		// 找出 li 中的超連結 href(#id)
		var $this = $(this),
			_clickTab = $this.find('a').attr('href');
		// 把目前點擊到的 li 頁籤加上 .active
		// 並把兄弟元素中有 .active 的都移除 class
		$this.addClass('active').siblings('.active').removeClass('active');
		// 淡入相對應的內容並隱藏兄弟元素
		$(_clickTab).stop(false, true).fadeIn().siblings().hide();
 
		return false;
	}).find('a').focus(function(){
		this.blur();
	});
	//按下搜尋回到第4個Tab
	$('#searchGIF').click(function() {
		var _showTab = 3;
		$('ul.tabs li').addClass('active').siblings('.active').removeClass('active');
		var $defaultLi = $('ul.tabs li').eq(_showTab).addClass('active');
		$($defaultLi.find('a').attr('href')).stop(false, true).fadeIn().siblings().hide();;
	});
});


</script>
</head>

<body background="img/bg.jpg" style="background-attachment:fixed">
	<center>
	<p style="font-size:30pt;font-family:Arial;width:900px"><b>Consistent Roof Geometry Encoding for 3D Building Model Retrieval Using Airborne LiDAR Point Clouds</b></p>
	
	<table rules="none" style="width:500px">
		<tr>
			<td>
				<p style="font-size:13pt;font-family:Arial;" align="center">
				<a href="mailto:powcyc@gmail.com">Yi-Chen Chen</a>
				</p>
			</td>
			<td>
				<p style="font-size:13pt;font-family:Arial;" align="center">
				<a href="mailto:pikadp2004@hotmail.com">Bo-Yi Lin</a>
				</p>
			</td>
			<td>
				<p style="font-size:13pt;font-family:Arial;" align="center">
				<a href="mailto:linhung@mail.ncku.edu.tw">Chao-Hung Lin</a>
				</p>
			</td>
		</tr>
	</table>
	<br>
	<form method="post" id="form2" name="form2" enctype="multipart/form-data" action="upload.php" target="frame_upload">
		<table rules="none">
			<tr>
				<td>
					<div id="upload_area" name="upload_area" align="center" style="width:900px;height:40px;border:1px #9999FF dashed;background-color:#D7E1FF;font-size: 16pt;font-family:Arial;line-height:37px;cursor: pointer;">Click to upload model (<b>xyz</b> and <b>obj</b> format)</div>
					<!--<div id="upload_area" name="upload_area" align="center" style="width:900px;height:40px;border:1px #9999FF dashed;background-color:#D7E1FF;font-size: 16pt;font-family:Arial;line-height:37px;cursor: pointer;">Click to upload model (<b>xyz</b> format)</div>-->
					<input type="file" name="file" id="file" style="display:none" accept='.obj,.xyz' />
					<!--<input type="file" name="file" id="file" style="display:none" accept='.xyz' />-->
				</td>
				<td>
					<img src="img/upload.gif" id="searchGIF" name="searchGIF" onclick="DoQuery()" style="cursor:hand" />
				</td>
			</tr>
		</table>
	</form>
	
	<iframe src="upload.php" id="frame_upload" style="display:none" name="frame_upload"></iframe>
	<div class="abgne_tab">
		<ul class="tabs">
			<li><a href="#tab1">Read Me</a></li>
			<li><a href="#tab2">Building Model DataBase</a></li>
			<li><a href="#tab3">Building Point Clouds</a></li>
			<li><a href="#tab4">Query Result</a></li>
		</ul>
		<div class="tab_container">
			<div id="tab1" class="tab_content">
				<h1>Abstract</h1>
				<p style="text-align: justify;text-justify:inter-ideograph;line-height:150%;">A 3D building model retrieval method using airborne LiDAR point clouds as input queries is introduced. Based on the concept of data reuse, available building models in the Internet that have geometric shapes similar to a user-specified point cloud query are retrieved and reused for the purpose of data extraction and building modeling. To retrieve models efficiently, point cloud queries and building models are consistently and compactly encoded by the proposed roof geometry encoding method. The encoding focuses on the geometries of building roofs, which are the most informative part of a building in airborne LiDAR acquisitions. Spatial histograms of geometric features that describe shapes of building roofs are utilized as shape descriptor, which introduces the properties of shape distinguishability, encoding compactness, rotation invariance, and noise insensitivity. These properties facilitate the feasibility of the proposed approaches for efficient and accurate model retrieval. Analyses on LiDAR data and building model databases and the implementation of web-based retrieval system, which is available at <a href="http://pcretrieval.dgl.xyz/">http://pcretrieval.dgl.xyz/</a>, demonstrate the feasibility of the proposed method to retrieve polygon models using point clouds.<br><br><b>Keywords</b>: point cloud encoding, spatial histogram, 3D model retrieval</p>
			</div>
			<div id="tab2" class="tab_content">
				<h1>Building Model Database</h1>
				<p align="right" style="font-size:9pt;font-family:Arial">database last updated: 2015-03-12 00:02:15</p>
				<p align="center">1,219,698 models have been collected.</p>
				<img src="img/database.png" width="800px">
			</div>
			<div id="tab3" class="tab_content">
				<h1>Examples Files</h1>
				<table width="700px" style="table-layout:fixed;border:1px #9999FF dashed;">
					<tr>
						<td colspan="3" align="center">
							<p style="font-size:16pt;font-family:Arial"><b>Point Clouds</b></p>
						</td>
					</tr>
					<tr>
						<td align="center"><a href="samples/Dept. of Geomatics.xyz"><img src="img/Dept. of Geomatics.png"></a></td>
						<td align="center"><a href="samples/Far Eastern Department Store.xyz"><img src="img/Far Eastern Department Store.png"></a></td>
						<td align="center"><a href="samples/Taipei 101.xyz"><img src="img/Taipei 101.png"></a></td>
					</tr>
					<tr>
						<td align="center">
							Dept. of Geomatics<br>
							32005 points, 1.06MB
						</td>
						<td align="center">
							Far Eastern Department Store<br>
							183719 points, 7.05MB
						</td>
						<td align="center">
							Taipei 101<br>
							55061 points, 2.29MB
						</td>
					</tr>
				</table>
				<br>
				<table width="700px" style="table-layout:fixed;border:1px #9999FF dashed; display:none">
					<tr>
						<td colspan="2" align="center">
							<p style="font-size:16pt;font-family:Arial"><b>Models</b></p>
						</td>
					</tr>
					<tr>
						<td><p></p></td>
						<td><p></p></td>
					</tr>
					<tr>
						<td align="center"><a href="samples/1_2.obj">1_2.obj</a></td>
						<td align="center"><a href="samples/1_3.obj">1_3.obj</a></td>
					</tr>
				</table>
			</div>
			<div id="tab4" class="tab_content">
				<p><span id='Qlog'>Ready</span></p>
				<span id='Qresult'></span>
			</div>
		</div>
	</div>
	<br>
	<p style="font-size:9pt;color:777777;font-family:Arial" align="center">
	Department of Geomatics, National Cheng-Kung University, Taiwan<br>
	Corresponding Author: <a href="mailto:linhung@mail.ncku.edu.tw">linhung@mail.ncku.edu.tw</a><br><br>
	&copy;Copyright Web-Designed by <b>Bo-Yi Lin</b><br></p>
	<form id='form1'>
	<input type='hidden' id='id' name='id' value='' />
	</form>
	</center>
</body>
</html>