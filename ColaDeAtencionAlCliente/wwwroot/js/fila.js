"use strict"

Vue.component('people-queue', {
    props: ['people'],
    template: `
        <div class="row">
        <div class="col-xs-1 col-sm-1 col-md-1 persona" v-for="person in people" :key="person.ID">
            <img src="/img/person.png" />
            <p>{{ person.Name }}</p>
        </div>
        </div>
    `
});

var app = new Vue({
    el: '#app',
    data: {
        queue1: [],
        queue2: [],
        connection: null,
        data: {
            cedula: "",
            nombre: ""
        }
    },
    methods: {
        addPerson: function (e) {
            this.connection.invoke("AddPerson", this.data.cedula, this.data.nombre).catch(function (err) {
                return console.error(err.toString());
            });
            this.data.cedula = "";
            this.data.nombre = "";
            e.preventDefault();
        }
    },
    watch: {
        queue1: function (response) {
            try {
                this.queue1 = JSON.parse(response);
            } catch (error) { }
        },
        queue2: function (response) {
            try {
                this.queue2 = JSON.parse(response);
            } catch (error) { }
        }
    },
    mounted: function () {
        var btn = this.$refs.btnAdd;
        btn.disabled = true;

        this.connection = new signalR.HubConnectionBuilder().withUrl("/personqueue").build();

        this.connection.start()
            .then(function () {
                btn.disabled = false;
            })
            .catch(function (err) {
                alert("Error de conexion");
                return console.error(err.toString());
            });

        this.connection.on('queue1', function (data) {
            console.log(data);
            app.queue1 = data;
        });

        this.connection.on('queue2', function (data) {
            console.log(data);
            app.queue2 = data;
        });
    }
});
