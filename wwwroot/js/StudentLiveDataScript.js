$(document).ready(function () {

    let connection = new signalR.HubConnectionBuilder().withUrl("/signalServer").build();
    connection.start();
    connection.on("RefreshStudentData", function () {
        GetAllStudents();
    })
    GetAllStudents();

    function GetAllStudents() {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', "/Home/GetAllStudents");

        xhr.onreadystatechange = function () {
            if (xhr.readyState == XMLHttpRequest.DONE) {
                if (xhr.status == 200) {
                    var studentList = JSON.parse(xhr.responseText);
                    console.log(studentList);
                    var setStudentList = document.getElementById('studentList');
                    setStudentList.innerHTML = "";

                    for (var i = 0; i < studentList.length; i++) {

                        var data = "<tr>" +
                            "<td>" + studentList[i].rollno + "</td>" +
                            "<td>" + studentList[i].fname + "</td>" +
                            "<td>" + studentList[i].lname + "</td>" +
                            "<td>" + studentList[i].city + "</td>" +
                            "<td>" + studentList[i].mobilenum + "</td>" +
                            "</tr>";

                        setStudentList.insertAdjacentHTML('beforeend', data);
                    }

                }
                else {
                    alert("Fail to Load Records..!!");
                }

            }
        }

        xhr.send();
    }
}) 





