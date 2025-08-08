$(function () {
    $('#imageFileInput, #Edit_Image').on('change', function () {
        const file = this.files && this.files[0];
        if (!file) return;
        $('.imagePreview').attr('src', URL.createObjectURL(file));
    });
    $('#profileModal,#categoryModal,#addModal,#EditModal,#UserEditModal').on('hidden.bs.modal', function () {
        location.reload();
    });
    $('#changePasswordForm').submit(function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        $.ajax({
            url: '/Auth/ChangePassword',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                $('#changePasswordAlert')
                    .removeClass('d-none alert-danger')
                    .addClass('alert-success')
                    .html(result);
                setTimeout(function () {
                    $('#changePasswordAlert').addClass('d-none').removeClass('alert-success').html('');
                }, 5000);
            },
            error: function (xhr) {
                $('#changePasswordAlert')
                    .removeClass('d-none alert-success')
                    .addClass('alert-danger')
                    .html(xhr.responseText || "Bir hata oluştu.")
                setTimeout(function () {
                    $('#changePasswordAlert').addClass('d-none').removeClass('alert-danger').html('');
                }, 5000);
            }
        });
    });
    $('#EditProfile').submit(function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        $.ajax({
            url: '/Home/EditProfile',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                $('#EditProfileAlert')
                    .removeClass('d-none alert-danger')
                    .addClass('alert-success')
                    .html(result);
                setTimeout(function () {
                    $('#EditProfileAlert').addClass('d-none').removeClass('alert-success').html('');
                }, 3000);
            },
            error: function (xhr) {
                $('#EditProfileAlert')
                    .removeClass('d-none alert-success')
                    .addClass('alert-danger')
                    .html(xhr.responseText || "Bir hata oluştu.")
                setTimeout(function () {
                    $('#EditProfileAlert').addClass('d-none').removeClass('alert-danger').html('');
                }, 3000);
            }
        });
    });
    $('#Category').submit(function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        $.ajax({
            url: '/Home/CategoryAdd',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                eval(result);
            },
            error: function (xhr) {
                eval(xhr.responseText || "Bir hata oluştu.");
            }
        });
    });
    $('#UserRole').submit(function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        $.ajax({
            url: '/Admin/RoleAdd',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                eval(result);
            },
            error: function (xhr) {
                eval(xhr.responseText || "Bir hata oluştu.");
            }
        });
    });
    $('#UserRoleEdit').submit(function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        $.ajax({
            url: '/Admin/RoleEdit',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                eval(result);
            },
            error: function (xhr) {
                eval(xhr.responseText || "Bir hata oluştu.");
            }
        });
    });
    $('#UserEdit').submit(function (e) {
        e.preventDefault();
        var formData = new FormData(this);

        $.ajax({
            url: '/Admin/UserEdit',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                if (result && result.trim().length) {
                    try {
                        eval(result);
                    } catch (err) {
                        console.error('Eval hatası:', err);
                    }
                }
            },
            error: function (xhr) {
                var txt = xhr.responseText || '';
                if (txt.trim().length) eval(txt);
            }
        });
    });
    $('#UserImageRemove').click(function (e) {
        e.preventDefault();

        var id = $('#Edit_Id').val();

        var formData = new FormData();
        formData.append("id", id);

        $.ajax({
            url: '/Admin/UserImageRemove',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                $('#ProfileImagePreview')
                    .attr('src', '/images/default.png')
                    .show();

                $('#Image').val('');
            },
            error: function (xhr) {
                Swal.fire('Hata', 'Resim silinirken bir sorun oluştu', 'error');
            }
        });
    });
    $('.edit-role-btn').click(function () {
        var id = $(this).data('id');
        $.ajax({
            url: '/Admin/RoleEdit',
            type: 'GET',
            data: { id: id },
            dataType: 'json',
            success: function (dto) {
                $('#Edit_Id').val(dto.id);
                $('#Edit_Name').val(dto.name);
                $('#Edit_Desciprtion').val(dto.desciprtion);

                var modal = new bootstrap.Modal($('#EditModal'));
                modal.show();
            },
            error: function () {
                Swal.fire('Hata', 'Rol bilgisi alınamadı', 'error');
            }
        });
    });
    $('.Useredit-role-btn').click(function () {
        var id = $(this).data('id');
        $.ajax({
            url: '/Admin/UserEdit',
            type: 'GET',
            data: { id: id },
            dataType: 'json',
            success: function (dto) {
                $('#Edit_Id').val(dto.id);
                $('#Edit_FullName').val(dto.fullName);
                $('#Edit_Email').val(dto.email);
                $('#Edit_RoleId').val(dto.roleId);

                if (dto.image) {
                    $('#ProfileImagePreview')
                        .attr('src', '/Uploads/Users/' + dto.image)
                        .show();

                    $('#Image').val(dto.image);
                } else {
                    $('#ProfileImagePreview')
                        .attr('src', '/images/default_user_image.png')
                        .show();
                    $('#Image').val('');
                }
                console.log(dto);
                var modal = new bootstrap.Modal($('#UserEditModal'));
                modal.show();
            },
            error: function () {
                Swal.fire('Hata', 'Rol bilgisi alınamadı', 'error');
            }
        });
    });
    const videos = document.querySelectorAll('video');
    videos.forEach(v => {
        v.volume = 0.3;

        v.addEventListener('play', () => {
            videos.forEach(other => {
                if (other !== v && !other.paused) {
                    other.pause();
                }
            });
        });
    });
    $('.list-group-item').on('click', function () {
        $('.list-group-item').removeClass('active');
        $(this).addClass('active');

        const selectedCategory = $(this).data('type');

        $('.col[data-category]').each(function () {
            const cardCategory = $(this).data('category');

            if (selectedCategory === 'Hepsi' || selectedCategory === cardCategory) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });
    $('#searchInput').on('keyup', function () {
        const keyword = $(this).val().toLowerCase();
        let found = false;

        $('.col[data-category]').each(function () {
            const title = $(this).find('.card-title').text().toLowerCase();
            const desc = $(this).find('.card-text').text().toLowerCase();

            if (title.includes(keyword) || desc.includes(keyword)) {
                $(this).show();
                found = true;
            } else {
                $(this).hide();
            }
        });

        $('#noResultsAlert').remove();

        if (!found) {
            const alertHtml = `
            <div class="col-12" id="noResultsAlert">
                <div class="alert alert-info rounded-4" role="alert">
                    <strong>Bilgi!</strong> Şu anda görüntülenecek video bulunmamaktadır.</a>
                </div>
            </div>`;
            $('#alertArea').html(alertHtml);
        }
    });
    $(document).ready(function () {
        let counter = 1;

        let table = $('#dataTable').DataTable({
            paging: true,
            lengthChange: false,
            searching: false,
            info: true,
            language: {
                info: "Total _TOTAL_ Records",
                infoEmpty: "No records to show",
                zeroRecords: "No data found in the table.",
                paginate: {
                    previous: "",
                    next: ""
                }
            }
        });

        function updateEntryInfo() {
            let count = table.rows({ search: 'applied' }).count();
            $('#entryInfo').text(`Total ${count} records`);
        }

        updateEntryInfo();

        $('#addForm').on('submit', function (e) {
            e.preventDefault();
            const name = $('#name').val();
            const description = $('#description').val();

            table.row.add([
                counter,
                name,
                description
            ]).draw();

            counter++;
            updateEntryInfo();
            $('#addModal').modal('hide');
            $('#addForm')[0].reset();
        });

        $('#customSearch').on('keyup', function () {
            table.search(this.value).draw();
            updateEntryInfo();
        });

        $('#entryCount').on('change', function () {
            table.page.len(this.value).draw();
        });
    });
});

$(function () {
    var saved = localStorage.getItem('theme') || 'light';
    var $html = $('html');
    var $icon = $('#themeIcon');

    $html.attr('data-bs-theme', saved);

    $icon
        .removeClass('bi-moon-fill bi-sun-fill')
        .addClass(saved === 'dark' ? 'bi-sun-fill' : 'bi-moon-fill');

    $('#themeToggle').on('click', function () {
        var current = $html.attr('data-bs-theme');
        var next = current === 'light' ? 'dark' : 'light';

        $html.attr('data-bs-theme', next);
        localStorage.setItem('theme', next);

        $icon
            .removeClass('bi-moon-fill bi-sun-fill')
            .addClass(next === 'dark' ? 'bi-sun-fill' : 'bi-moon-fill');
    });
});

function Delete(urlStr, id) {

    Swal.fire({
        title: 'You are about to delete this record!',
        text: "All records related to this record will also be deleted!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#333',
        confirmButtonText: 'Yes, Please!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: urlStr + id,
                type: "Get",
                contentType: 'application/json; charset=utf-8',
                dataType: "json",
                success: function (data, textStatus, jQxhr) {
                    if (data === "selfDeleted") {
                        window.location.href = "/Login";
                    }
                    else if (data === "success") {
                        window.location.reload();
                    }
                    else {
                        Swal.fire("Error", data.message || "Unexpected error", "error");
                    }
                },
                error: function (jqXhr, textStatus, errorThrown) {
                    console.log(errorThrown);
                }
            });
        }
    });
}