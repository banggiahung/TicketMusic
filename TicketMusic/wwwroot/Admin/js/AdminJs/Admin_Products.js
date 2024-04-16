var Admin_video = new Vue({
    el: '#Admin_video',
    data: {
        dataItems: [],
        dataProductsItems: [],
        dataSubItems: [],
        dataGiaoVien: [],
        dataCategory: [],
        dataCm: [],



        filteredSubCategories: [],
        filteredSubCategoriesEdit: [],
        dataBrands: [],
        dataAge: [],
        dataItemsRim: [],
        dataTool: [],
        dataSize: [],
        dataColor: [],
        VideoFree: [],
        ListSiteMapNone: [],

        id: "",
        CategoryID: 0,
        CateEditID: 0,
        SubCategoryID: 0,
        chuDeID: 0,
        giaoVienID: 0,
        freeMain: 0,
        timeVideo: 0,
        linkVideo: "",
        linkBt: "",
        linkRs: "",

        ToolId: 0,
        RimID: 0,
        brandsID: 0,
        clbID: 0,
        sizeID: 0,
        OrderNumber: 0,
        SizeArray: [],

        CodeProduct: "",
        Titile: "",
        PriceMain: 0,
        DisPrice: 0,
        ImageMain: "",
        ShortDes: "",
        Details: "",
        LinkDownload: "",

        imageFile: null,
        previewImage: null,
        uploadedImage: null,
        imageProducts: "",
        SubCategoryName: "",
        Slug: "",

        selectedFiles: null,
        imagesPreview: [],
        fullImg: [],
        processedFiles: [],
        listNumber: [],
        suggestions: [],
        originalDataColor: [],

        ckName: "",
        editor: "",


        showTable1: true,
        chooseCate: false,
        previewVideo: null,

        checkButton: false,

        listUrl: []
       
    },
    computed: {
        computedSlug() {
            return this.generateSlug(this.Titile);
        }

    },
    created() {
        EventBus.$on('listUrlReceived', data => {
            this.listUrl = data;
        });

    },
    watch: {
       
        SubCategoryID(newVal) {
            console.log(newVal)
        },
        //computedSlug(newVal) {
        //    this.Slug = newVal;
        //},
        CategoryID(newVal, oldVal) {
            if (newVal !== null && newVal !== 0) {
                this.chooseCate = true;

              
            } else {
                this.chooseCate = false;
            }
                this.loadSubCategories();

        },
        CateEditID(newVal, oldVal) {
            if (newVal !== oldVal) {
                this.loadWhenEdit();

            } else {
                this.filteredSubCategoriesEdit = [...this.filteredSubCategories];
            }


        },
        processedFiles: {
            handler(newValue) {
                if (newValue && newValue.length) {
                    for (let i = 0; i < newValue.length; i++) {
                        console.log(newValue[i]);
                    }
                }
            },
            deep: true
        },
        ProductNumber: function (newVal, oldVal) {
            this.updateAvailableColors();
        },
        ckName(vl) {
            console.log(vl);
        }
    },
    beforeDestroy() {
        if (this.editor) {
            this.editor.destroy();
        }
    },
    mounted() {
    
       
       
        $('#preloader').fadeIn();
        this.loadSubItems();
       
        axios.get("/Admin/Course/GetAllCourseTitle")
            .then((response) => {
                this.dataSubItems = response.data;
            });
        axios.get("/Admin/GiaoVien/GetAllGiaoVien")
            .then((response) => {
                this.dataGiaoVien = response.data;

            });
        axios.get("/Admin/ChuDe/GetAllChuDe")
            .then((response) => {
                this.dataCm = response.data;

            });
        $(document).ready(function () {
            $('#selectChuDe').select2();
            $('#selectGiaoVien').select2();

            $('#exampleModal').on('show.bs.modal', function () {
              
                $('#selectChuDe').select2({
                    dropdownParent: $('#exampleModal')
                }); 
                $('#selectGiaoVien').select2({
                    dropdownParent: $('#exampleModal')
                });
              
            })

            $('#exampleModal').on('hidden.bs.modal', function () {
                $('#selectChuDe').select2('destroy');
                $('#selectGiaoVien').select2('destroy');
               
            })
        });

    },
    methods: {
        clickButton() {
            this.checkButton = !this.checkButton;
        },
        onFileChangeVideo(event) {
            const selectedVideo = event.target.files[0];
            if (selectedVideo) {
                this.previewVideo = URL.createObjectURL(selectedVideo);
            } else {
                this.previewVideo = null;
            }
        },
        loadSubCategories() {
            this.dataSubItems = this.filteredSubCategories.filter(item => item.categoryID === this.CategoryID);
        },
        loadWhenEdit() {
            this.filteredSubCategories = this.filteredSubCategoriesEdit.filter(item => item.categoryID === this.CateEditID);

        },
        mapDataSubItems(subCateID) {
            const foundItem = this.filteredSubCategories.find(item => item.id === subCateID);

            if (foundItem) {
                this.SubCategoryID = foundItem.id;
            }
        },
        toggleTables() {
            this.showTable1 = !this.showTable1;
            if (!this.showTable1) {

                $('#product_table').DataTable().destroy();

            } else {
                this.loadSubItems();

            }
        },
        AddToSitemap(url) {
            try {
                const formData = new FormData();
                formData.append('url', url);
                axios.post('/Admin/WebConfig/AddToSitemap', formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    });
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công',
                    text: 'Đã lưu thành công',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();

                    }
                });
            }
            catch (error) {
                console.error(error);
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Đã có lỗi xảy ra',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();
                    }
                });
            }
           
        },
        loadSubItems() {
            let currentPage = 0;
            if ($.fn.DataTable.isDataTable('#video_table')) {
                currentPage = $('#video_table').DataTable().page();
                $('#video_table').DataTable().destroy();
            }

            axios.get("/Admin/Video/GetAllVideo")
                .then((response) => {
                    this.dataItems = response.data;
                    this.VideoFree = this.dataItems.filter(item => item.freeActive === true);
                    this.VideoFree.forEach(video => {
                        const url = `/video/${video.courses.mainSubCourse.slugMain}/${video.courses.slug}/${video.videoID}`;
                        video.url = url;
                    });
                    const videoUrls = this.VideoFree.map(video => `/video/${video.courses.mainSubCourse.slugMain}/${video.courses.slug}/${video.videoID}`);

                    videoUrls.forEach(url => {
                        if (!this.listUrl.includes(url)) {
                            this.ListSiteMapNone.push(url);
                        }
                    });

                    $('#preloader').fadeOut();

                    return Promise.resolve();
                })
                .then(() => {
                    const table = $("#video_table").DataTable({
                        deferRender: true,

                        ...this.$globalConfig.createDataTableConfig(),
                        'columnDefs': [{
                            'targets': [-1],
                            'orderable': false,
                        }],
                        searching: true,
                        iDisplayLength: 7,
                        "ordering": false,
                        lengthChange: false,
                        aaSorting: [[0, "desc"]],
                        aLengthMenu: [
                            [5, 10, 25, 50, 100, -1],

                            ["5 dòng", "10 dòng", "25 dòng", "50 dòng", "100 dòng", "Tất cả"],
                        ]

                    });
                    if (currentPage !== 0) {
                        table.page(currentPage).draw('page');
                    }
                });
        },
        generateSlug(text) {

            let str = text.normalize('NFD').replace(/[\u0300-\u036f]/g, '');

            const replacements = {
                "đ": "d",
                "Đ": "D"
            };

            str = str
                .replace(/đ/g, 'd')
                .replace(/Đ/g, 'D')
                .toLowerCase()
                .replace(/ /g, '-')
                .replace(/[^\w-]+/g, '');

            console.log("Final slug:", str);

            return str;
        },

        previewFiles(event) {
            const newFilesArray = Array.from(event.target.files);
            this.processedFiles = newFilesArray;
            for (let i = 0; i < newFilesArray.length; i++) {
                const imgSrc = URL.createObjectURL(newFilesArray[i]);
                this.imagesPreview.push(imgSrc);
            }
        },
        removeImage(index) {
            this.imagesPreview.splice(index, 1);
            this.processedFiles.splice(index, 1);
        },
        removeImageMain(index, data) {
            this.fullImg.splice(index, 1);
            if (data) {
                const formData = new FormData();
                formData.append('id', data.id);
                axios.post('/Admin/Products/Delete', formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    });
            }
        },


        onFileChange(event) {
            this.imageFile = event.target.files[0];
            this.previewImage = URL.createObjectURL(this.imageFile);
            this.uploadedImage = null;
        },
        formatDate(date) {
            const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
            return date.toLocaleDateString('vi-VN', options);
        },
        formatCurrency(amount) {
            const formatter = new Intl.NumberFormat('vi-VN', {
                style: 'currency',
                currency: 'VND'
            });

            return formatter.format(amount);
        },
        async addProducts() {
            try {
              
                const videoFile = this.$refs.FileName.files[0];
                const formData = new FormData();

                formData.append('Title', this.Titile);
                formData.append('SlugVideo', this.Slug);
                formData.append('Duration', this.timeVideo);
                formData.append('ChuDeID', this.chuDeID);
                formData.append('TeacherID', this.giaoVienID);
                formData.append('freeMain', this.freeMain);
                formData.append('LinkBt', this.linkBt);
                formData.append('LinkResult', this.linkRs);
                formData.append('OrderNumber', this.OrderNumber);
                formData.append('CourseID', this.SubCategoryID);
                if (videoFile) {
                    formData.append('video', videoFile);
                } else {
                    formData.append('URL', this.linkVideo);

                }

                await axios.post('/Admin/Video/AddVideo', formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    });
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công',
                    text: 'Đã lưu thành công',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();
                    }
                });
            } catch (error) {
                console.error(error);
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Đã có lỗi xảy ra',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();
                    }
                });
            }
        },
        getItemsById(id) {
            axios.get(`/Admin/Video/GetByIDVideo/${id}`)
                .then((response) => {
                    this.id = response.data.videoID;
                    this.Titile = response.data.title;
                    this.CateEditID = response.data.categoryID;
                    this.SubCategoryID = response.data.courseID;
                    this.chuDeID = response.data.chuDeID;
                    this.giaoVienID = response.data.teacherID;
                    this.freeMain = response.data.freeActive ? 1 : 0;
                    this.timeVideo = response.data.duration;
                    this.linkVideo = response.data.url;
                    this.linkBt = response.data.linkBt;
                    this.linkRs = response.data.linkResult;
                    this.Slug = response.data.slugVideo;
                    this.OrderNumber = response.data.orderNumber;
                    this.mapDataSubItems(response.data.subCateID);
                    $('#selectChuDeEdit').val(this.chuDeID).trigger('change');
                    $('#selectGiaoVienEdit').val(this.giaoVienID).trigger('change');
                    $('#selectChuDeEdit').select2();
                    $('#selectGiaoVienEdit').select2();

                    return Promise.resolve();
                });
        },
        resetData() {
            this.id = "";
            this.Titile = "";
            this.Slug = "";
            this.CateEditID = 0;
            this.SubCategoryID = 0;
            this.freeMain = 0;
            this.timeVideo = 0;
            this.giaoVienID = 0;
            this.chuDeID = 0;
            this.OrderNumber = 0;
            this.linkVideo = "";
            this.linkBt = "";
            this.linkRs = "";
            this.previewVideo = null;
           

        },
        async editProducts() {
            const currentThis = this;

            try {

                if (this.SubCategoryID === 0 || this.CateEditID === 0 || this.linkVideo === "" || this.Titile === "" ) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Vui lòng nhập đủ các trường',
                        confirmButtonText: 'OK'
                    })
                    return;
                }
                if (this.Slug === "") {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Vui lòng nhập slug',
                        confirmButtonText: 'OK'
                    })
                    return;
                }
                const videoFile = this.$refs.FileName1.files[0];

                const formData = new FormData();
                formData.append('VideoID', currentThis.id);
                formData.append('Title', currentThis.Titile);
                formData.append('SlugVideo', currentThis.Slug);
                formData.append('Duration', currentThis.timeVideo);
                formData.append('ChuDeID', currentThis.chuDeID);
                formData.append('TeacherID', currentThis.giaoVienID);
                formData.append('freeMain', currentThis.freeMain);
                formData.append('LinkBt', currentThis.linkBt);
                formData.append('LinkResult', currentThis.linkRs);
                formData.append('OrderNumber', currentThis.OrderNumber);
                formData.append('CourseID', currentThis.SubCategoryID);
                if (videoFile) {
                    formData.append('video', videoFile);
                } else {
                    formData.append('URL', currentThis.linkVideo);

                }

                await axios.post('/Admin/Video/UpdateVideo', formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    });
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công',
                    text: 'Đã lưu thành công',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        this.loadSubItems();
                    }
                });
            } catch (error) {
                console.error(error);
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Đã có lỗi xảy ra',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();

                    }
                });
            }
        },
        getItemsByIdDelete(id) {
            axios.get(`/Admin/Video/GetByIDVideo/${id}`)
                .then((response) => {
                    this.id = response.data.videoID;

                    if (this.id != null) {
                        Swal.fire({
                            title: 'Xóa sản phẩm',
                            text: 'Bạn có chắc chắn muốn xóa',
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Đồng ý',
                            cancelButtonText: 'Không!!!'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                const formData = new FormData();
                                formData.append('VideoID', this.id);
                                axios.post('/Admin/Video/DeleteVideo', formData, {
                                    headers: {
                                        'Content-Type': 'application/x-www-form-urlencoded'
                                    }
                                }).then(response => {
                                    Swal.fire({
                                        icon: 'success',
                                        title: 'Thành công',
                                        text: 'Đã thành công',
                                        confirmButtonText: 'OK',
                                    }).then((result) => {
                                        if (result.isConfirmed) {
                                            this.loadSubItems();

                                        }
                                    });

                                }).catch(error => {
                                    Swal.fire({
                                        icon: 'error',
                                        title: 'Lỗi',
                                        text: 'Đã có lỗi xảy ra vui lòng thử lại',
                                        confirmButtonText: 'OK'
                                    });
                                });
                            } else {
                                return;
                            }
                        });
                    }
                }).catch((error) => {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Đã có lỗi xảy ra vui lòng thử lại',
                        confirmButtonText: 'OK'
                    });
                })
        },
    }
})