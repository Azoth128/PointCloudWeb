<template>
  <div class="map-grid-container">
    <div id="map-settings">
      <div class="map-settings-internal">
        <h3>Point Clouds</h3>

        <ul v-for="item in cloudItems" :key="item">
          <li>
            <ScanItem :item="item"/>
          </li>
        </ul>
      </div>
    </div>
    <div id="map">
      <PotreeViewer/>
    </div>
  </div>
</template>
<script>
import ScanItem from "@/components/ScanItem";
import PotreeViewer from "@/components/PotreeViewer";

export default {
  created() {
    this.$store.dispatch("pci/loadPointClouds");
  },
  computed: {
    cloudItems() {
      return this.$store.state.pci.pointClouds;
    },
  },
  components: {PotreeViewer, ScanItem},
};
</script>

<style scoped>

PotreeViewer {
  width: 100%;
  height: 100%;
  border: 0;
}
.map-grid-container {
  text-align: left;
}

ul {
  padding-left: 0;
}

li {
  font-size: 1.4em;
  list-style: none;
}

p {
  display: inline-block;
  margin: 0 0 0 10px;
}

h3 {
  margin: 0;
}

a {
  text-decoration: none;
}

#map-settings {
  grid-area: right;
  border-left: 1px solid black;
  height: 100%;
  overflow-y: auto;
}

.map-settings-internal {
  padding: 20px;
}

#map {
  grid-area: main;
  overflow-y: auto;
}

.map-grid-container {
  display: grid;
  grid-template-areas: "main right";
  grid-template-columns: calc(100vw - 325px) 325px;
  height: calc(
      100vh - 2em - 20px - 1px
  ); /*viewport height - height of navbar-button - padding - borderline*/
  grid-gap: 0;
  padding: 0;
}
</style>